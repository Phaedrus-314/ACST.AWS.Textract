namespace ACST.AWS.TextractViewer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using ACST.AWS.Common;
    using ACST.AWS.Textract.Model;
    using Model = Amazon.Textract.Model;

    using Cyotek.Windows.Forms;
    using ACST.AWS.Common.OCR;
    using ACST.AWS.TextractClaimMapper.ADA;
    using Amazon.Textract.Model;
    using System.Diagnostics;
    using System.Drawing.Drawing2D;
    using System.Resources;

    internal partial class ViewerJpg
        : BaseForm
    {

        bool showDebugControls = false;
        List<GraphicsLine> TestLines;

        #region Fields & Properties

        bool isMouseDown = false;
        float imageRotationDegrees = 0F;
        bool ShowHighlights = true;

        bool HighlightOnTab = Properties.Settings.Default.HighlightOnTab;

        GraphicsHandle selectedHandle;

        List<RectangleF> fieldRectangles;

        enum EditMode
        {
            EditField,
            EditTable,
            EditCoordinate,
            Drawing,
            Validation
        }

        enum HighlightMode
        {
            None,
            OCR,
            Matched
        }

        EditMode editMode;
        bool cacheLoading;

        bool drawing;
        System.Drawing.Point drawingStartPos;
        System.Drawing.Point drawingCurrentPos;

        SizeF scaledGauge { get; set; }

        SizeF scaledFontSize { get; set; }

        Model.BoundingBox currentFieldBoundingBox;
        List<Rectangle> handDrawnRectangles = new List<Rectangle>();
        List<Rectangle> namedCoordinatesRectangles = new List<Rectangle>();
        GraphicsTextDictionary graphicsTextDictionary = new GraphicsTextDictionary();
        
        Dictionary<string, string> namedCoordinatesFileNameDictionary = new Dictionary<string, string>();

        int _currentFieldTabOrder;

        int currentFieldTabOrder
        {
            get
            {
                return _currentFieldTabOrder;
            }
            set
            {
                if (value < this.TextractClaimImage.TextractDocument.NamedCoordinates.MinTabOrder)
                    _currentFieldTabOrder = this.TextractClaimImage.TextractDocument.NamedCoordinates.MinTabOrder;
                else if (value > this.TextractClaimImage.TextractDocument.NamedCoordinates.MaxTabOrder)
                    _currentFieldTabOrder = this.TextractClaimImage.TextractDocument.NamedCoordinates.MaxTabOrder;
                else
                    _currentFieldTabOrder = value;

                txtCurrentFieldNo.Text = _currentFieldTabOrder.ToString();
            }
        }


        bool HighlightMatchedClaimsData { get; set; }
        //HighlightMode HighlightClaimsData { get; set; }

        public static Dictionary<string, ContextData> ContextDataCache => new Dictionary<string, ContextData>();

        // ToDo: Make this a cache
        ContextData FieldContextData { get; set; }

        TextractElements SelectedTextractElements { get; set; }

        TextractViewerAdornments SelectedTextractViewerAdornments { get; set; }

        internal TextractClaimImage TextractClaimImage { get; set; }

        PointF ImageRelativeCenter => imageBox.GetOffsetPoint(TextractClaimImage.AbsoluteImageCenter(this.TextractClaimImage.SourceImage));
        #endregion

        #region Constructors
        public ViewerJpg()
        {
            InitializeComponent();
        }

        public ViewerJpg(TextractClaimImage textractClaimImage)
        {
            cacheLoading = true;

            this.TextractClaimImage = textractClaimImage;

            InitializeComponent();

            mainSplitContainer.Panel2Collapsed = true;
            mainSplitContainer.Panel2.Hide();

            this.KeyDown += new KeyEventHandler(ViewerJpg_KeyDown);
        }
        #endregion

        #region Conditional 
        [Conditional("DEBUG")]
        void ShowDebugControls()
        {
            tsButton_Test1.Visible = showDebugControls;
            btnTestClear.Visible = showDebugControls;
            textBox2.Visible = showDebugControls;
            label12.Visible = showDebugControls;
        }
        #endregion

        #region Form Overrides

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //this.MinimumSize = new System.Drawing.Size(this.Width, this.Height);
            //this.MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height); //, (int)System.Windows.SystemParameters.PrimaryScreenHeight);
            //this.Height = 650;
            //this.Width = 1400;

            if (Properties.Settings.Default.MaximizeOnOpen)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;

            

            ShowDebugControls();

            if (this.TextractClaimImage != null)
                LoadTextractImage();

            cacheLoading = false;

            currentFieldTabOrder = 1;
            editMode = EditMode.EditField;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            //tsslFormSize.Text = $"Size: [{this.Width},{this.Height}]";
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (Properties.Settings.Default.SaveSettingsOnExit)
                Properties.Settings.Default.Save();

            // Dispose ImageBox image
            if (this.imageBox.Image != null)
            {
                var existingFile = this.imageBox.Image;
                imageBox.Image = null;
                existingFile?.Dispose();
            }

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
        if (changingTextboxConfidence)
                return base.ProcessCmdKey(ref msg, keyData);

            switch (keyData)
            {
                case Keys.Control | Keys.R:
                    ShowContextPanel(mainSplitContainer.Panel2Collapsed);
                    break;
                case Keys.Home | Keys.Control:
                    imageBox.ScrollTo(0, 0);
                    //ToolStripSplitButton_Highlights_ButtonClick(sender, e);
                    break;
                case Keys.Tab | Keys.Shift:
                    btnPrevious.PerformClick();
                    break;
                case Keys.Tab:
                    btnNext.PerformClick();
                    break;

                case Keys.Control | Keys.Tab:
                    this.SelectNextControl(this.ActiveControl, true, true, true, true);
                    break;
                case Keys.Control | Keys.Tab | Keys.Shift:
                    this.SelectNextControl(this.ActiveControl, false, true, true, true);
                    break;

                default:
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region Form Initialize

        int DropDownWidth(ToolStripComboBox ctrl)
        {
            int maxWidth = 200, temp = 0;
            foreach (var obj in ctrl.Items)
            {
                temp = TextRenderer.MeasureText(obj.ToString(), ctrl.Font).Width;
                if (temp > maxWidth)
                    maxWidth = temp;
            }
            return maxWidth;
        }

        void FillZoomLevelSelector()
        {
            ComboBox_ZoomLevel.Items.Clear();

            foreach (int zoom in imageBox.ZoomLevels)
            {
                ComboBox_ZoomLevel.Items.Add(string.Format("{0}%", zoom));
            }
        }

        void FillNamedCoordinatesSelector()
        {
            namedCoordinatesFileNameDictionary.Clear();

            foreach (var value in EnumExtensions.GetValues<ClaimFormType>())
            {
                namedCoordinatesFileNameDictionary.Add(value.ToString(), NamedCoordinates.GetFileNameFromFormType(value));
            }

            ComboBox_NamedCoordinates.Items.Clear();
            ComboBox_NamedCoordinates.Items.Add(ClaimFormType.ADA2006.ToString());
            ComboBox_NamedCoordinates.Items.Add(ClaimFormType.ADA2012.ToString());
            //ComboBox_NamedCoordinates.Items.Add(ClaimFormType.ADA2019.ToString());


            ComboBox_NamedCoordinates.SelectedIndex = ComboBox_NamedCoordinates.FindStringExact(TextractClaimImage.MetaData.FormType.ToString());

            //ComboBox_NamedCoordinates.SelectedIndex = 1;
            ComboBox_NamedCoordinates.AutoSize = false;
            ComboBox_NamedCoordinates.Width = 100; // DropDownWidth(ComboBox_NamedCoordinates) * 2;
        }

        void LoadTextractImage()
        {

            this.SelectedTextractElements = TextractElements.Page | TextractElements.FieldValue | TextractElements.Table;

            this.HighlightMatchedClaimsData = true;

            SetToolbarButtons();

            imageBox.Image = this.TextractClaimImage.SourceImage;

            this.FillZoomLevelSelector();

            this.FillNamedCoordinatesSelector();

            imageBox.SelectionMode = ImageBoxSelectionMode.Rectangle;

            imageBox.AutoCenter = true;
            imageBox.AllowClickZoom = false;
            imageBox.AllowDoubleClick = true;

            this.scaledGauge = imageBox.GetScaledSize(new SizeF(TextractClaimImageStyle.DefaultGauge, 0));
            this.scaledFontSize = imageBox.GetScaledSize(new SizeF(TextractClaimImageStyle.DefaultFont.Size, 0));


            SetPatientToInsured();

            DisplayClaimTableContext();
        }

        void ReloadImage(string coordinatesId)
        {
            if (cacheLoading)
                return;

            var nc = new ADANamedCoordinates(namedCoordinatesFileNameDictionary[coordinatesId]);

            this.TextractClaimImage.Cache.ReBuildClaim(nc);

            SetPatientToInsured();

            DisplayClaimTableContext();
            
            this.Refresh();
        }

        void SetToolbarButtons()
        {
            //ToolStripSplitButton_Highlights.Text = this.HighlightClaimsData == HighlightMode.None ? "Highlights Off" : "Highlights On";

            //if (this.HighlightClaimsData == HighlightMode.None)
            //    ToolStripSplitButton_Highlights.DropDownItems["matchedFieldsToolStripMenuItem"].Select();
            //else
            //    ToolStripSplitButton_Highlights.DropDownItems["allOCRFieldsToolStripMenuItem"].Select();

            //toolStripButton_MatchedFields.Checked = HighlightMatchedClaimsData;

            ToolStripMenuItem_Page.Checked = SelectedTextractElements.HasFlag(TextractElements.Page);
            ToolStripMenuItem_Field.Checked = SelectedTextractElements.HasFlag(TextractElements.Field);
            ToolStripMenuItem_FieldKey.Checked = SelectedTextractElements.HasFlag(TextractElements.FieldKey);
            ToolStripMenuItem_FieldValue.Checked = SelectedTextractElements.HasFlag(TextractElements.FieldValue);
            ToolStripMenuItem_Table.Checked = SelectedTextractElements.HasFlag(TextractElements.Table);
        }
        #endregion

        #region ContextData Metohds

        public void CreateValidationResultContext()
        {
            BindingSource source = new BindingSource();

            dgvValidation.Columns.Clear();
            dgvValidation.Rows.Clear();
            dgvValidation.DataSource = null;
            dgvValidation.Refresh();
            lbValidationResults.Items.Clear();

            int columnPosition = 0;
            dgvValidation.AutoGenerateColumns = false;
            dgvValidation.AutoSize = true;
            dgvValidation.RowHeadersVisible = false;
            dgvValidation.DataSource = source;

            //DataGridViewColumn(dgvValidation, "Required", new DataGridViewTextBoxColumn(), ++columnPosition);
            DataGridViewColumn(dgvValidation, "Group", new DataGridViewTextBoxColumn(), ++columnPosition);
            DataGridViewColumn(dgvValidation, "Name", new DataGridViewTextBoxColumn(), ++columnPosition);
            DataGridViewColumn(dgvValidation, "Value", new DataGridViewTextBoxColumn(), ++columnPosition);
            DataGridViewColumn(dgvValidation, "Type", new DataGridViewTextBoxColumn(), ++columnPosition);
            DataGridViewColumn(dgvValidation, "MappedPropertyInfo", new DataGridViewTextBoxColumn(), ++columnPosition);

            if (this.TextractClaimImage.ValidationResults?.Count() == 0)
            {
                lbValidationResults.Items.Add("Validation successful, ready for Export...");
            }
            else
            {
                foreach (var result in this.TextractClaimImage?.ValidationResults)
                {
                    lbValidationResults.Items.Add($"{result.Result}{Environment.NewLine}");

                    //if (result.Group != "Line")
                    //{
                    var field = this.TextractClaimImage.Page.Form.Fields.FirstOrDefault(f => f.Match == result.Name);

                    if (field == null)
                    {
                        //var coord = this.TextractClaimImage.Cache.TextractDocument.NamedCoordinates.SingleOrDefault(n => n.Name == result.Name);
                        var coord = this.TextractClaimImage.Cache.TextractDocument.NamedCoordinates.FirstOrDefault(n => n.Name == result.Name);
                        if (coord == null)
                            coord = this.TextractClaimImage.Cache.TextractDocument.NamedCoordinates.FirstOrDefault(n => n.GroupName == result.Name);

                        field = new Textract.Model.Field(coord);
                    }
                    this.FieldContextData = ContextData.GetContext(this.TextractClaimImage, field, TextractElements.FieldValue | TextractElements.FieldKey);
                    //}
                    DisplayValidationResultsContext(source, result.Group);
                }

                if (source.Count > 0)
                {
                    dgvValidation.AutoSize = true;
                    //dgvValidation.Columns["Required"].Width = Convert.ToInt32(dgvValidation.Width * .1);
                    dgvValidation.Columns["Group"].Width = Convert.ToInt32(dgvValidation.Width * .10);
                    dgvValidation.Columns["Name"].Width = Convert.ToInt32(dgvValidation.Width * .38);
                    dgvValidation.Columns["Value"].Width = Convert.ToInt32(dgvValidation.Width * .45);
                    dgvValidation.Columns["Group"].ReadOnly = true;
                    dgvValidation.Columns["Name"].ReadOnly = true;
                    dgvValidation.Columns["Value"].ReadOnly = false;
                    dgvValidation.Columns["Type"].Visible = false;
                    dgvValidation.Columns["MappedPropertyInfo"].Visible = false;
                }
            }
        }

        void DisplayValidationResultsContext(BindingSource source, string group, bool highlightField = false)
        {
            ShowContextPanel();
            tabControl.SelectTab("tabPage_Validation");

            //if (this.FieldContextData.ElementType == "Table" & this.FieldContextData.CellElements?.Count() > 0)
            //{
            //    tabControl.SelectTab("tabPage_EditTable");
            //}
            //else
            if (this.FieldContextData.Element != null)
            {
                switch (this.FieldContextData.PropertyType)
                {
                    case "String":
                        source.Add(new ContextItem
                        {
                            Group = group,
                            Type = this.FieldContextData.PropertyType,
                            MappedPropertyInfo = FieldContextData.MappedPropertyInfo,
                            Name = FieldContextData.MappedPropertyAttributeInstanceName,
                            Value = FieldContextData.MappedPropertyInfo?.GetValue(this.TextractClaimImage.Claim) as string
                        });
                        break;

                    case "Field":
                        if (this.FieldContextData.MappedPropertAttribute.Name == "GroupFieldAttribute")
                        {
                            DataGridViewColumn(dgvValidation, "Name", new DataGridViewTextBoxColumn(), 1);
                            dgvValidation.Columns.Add(CreateComboBoxWithEnums(FieldContextData.MappedGroupPropertyInfo.PropertyType, 2));
                            DataGridViewColumn(dgvValidation, "Type", new DataGridViewTextBoxColumn(), 3);
                            DataGridViewColumn(dgvValidation, "MappedPropertyInfo", new DataGridViewTextBoxColumn(), 4);

                            source.Add(new ContextItem
                            {
                                Group = group,
                                Type = this.FieldContextData.MappedGroupPropertyInfo.PropertyType.FullName,
                                MappedPropertyInfo = this.FieldContextData.MappedGroupPropertyInfo,
                                Name = FieldContextData.MappedPropertyGroupName,
                                Value = FieldContextData.MappedGroupPropertyInfo?.GetValue(this.TextractClaimImage.Claim)
                            });
                        }
                        else
                        {
                            //this.FieldContextData.ComositeValueProperties
                            this.FieldContextData.CompositeValuePropertyItems
                                .ToList().ForEach(p =>
                                    source.Add(new ContextItem
                                    {
                                        Group = group,
                                        Type = this.FieldContextData.PropertyType,
                                        MappedPropertyInfo = p.PropertyInfo,
                                        Name = p.PropertyInfo.Name,
                                        Value = p.PropertyInfo.GetValue(this.TextractClaimImage.Claim) as string
                                    }));
                        }
                        break;

                    default:
                        if (this.FieldContextData.PropertyType.IsNotNullOrEmpty())
                            Logger.TraceWarning("Claim propery type: '{0}', does not yet support update.", this.FieldContextData.PropertyType);
                        break;
                }
            }
        }

        DataGridViewComboBoxColumn CreateComboBoxWithEnums(Type type, int displayIndex = -1)
        {
            if (type == null)
                return null;

            DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn();
            if (displayIndex != -1)
                combo.DisplayIndex = displayIndex;
            combo.DataSource = Enum.GetValues(type);
            combo.ValueType = type;
            combo.DataPropertyName = "Value";
            combo.Name = "Value";
            return combo;
        }

        void DisplayOCRContextValues()
        {
            if (mainSplitContainer.Orientation == Orientation.Vertical)
                txtOCROriginalKey.Text = FieldContextData.OCRFieldName?.Truncate(60, true); // Form field name
            else
                txtOCROriginalKey.Text = FieldContextData.OCRFieldName;

            txtOCROriginalValue.Text = FieldContextData.OCRFieldValue;

            //lblOCROriginalKeyConfidence.Text = $"{FieldContextData.OCRFieldMinConfidence}%";
            txtOCROriginalKeyConfidence.Text = $"{FieldContextData.OCRFieldMinConfidence}%";
            lblOCROriginalValueConfidence.Text = $"{FieldContextData.OCRFieldValueConfidence}%";
            changingTextboxConfidence = false;

            if (FieldContextData.IsFieldRequired)
            {
                panelValueConfidence.BackColor = FieldConfidenceHighlightColor(FieldContextData.OCRFieldValueConfidence, FieldContextData.OCRFieldMinConfidence, FieldContextData.IsApproved);
                if (panelValueConfidence.BackColor == TextractClaimImageStyle.ocrConfidenceFail)
                    lblOCROriginalValueConfidence.ForeColor = Color.White;
                else
                    lblOCROriginalValueConfidence.ForeColor = Color.Black;
            }
            else
            {
                panelValueConfidence.BackColor = SystemColors.ButtonFace;
                lblOCROriginalValueConfidence.ForeColor = Color.Black;
            }
        }

        void DisplayClaimFieldContext(bool highlightField = false)
        {
            if (this.FieldContextData.ElementType == null)
                return;

            if (this.FieldContextData.ElementType == "Table" 
                & this.FieldContextData.CellElements?.Count() > 0)
            {
                tabControl.SelectTab("tabPage_EditTable");
            }
            else
            if (this.FieldContextData.Element != null)
            {
                this.namedCoordinatesRectangles.Clear();
                this.currentFieldBoundingBox = null;

                BindingSource source = new BindingSource();

                dgvEdit.Columns.Clear();
                dgvEdit.Rows.Clear();
                dgvEdit.DataSource = null;
                dgvEdit.Refresh();

                dgvEdit.AutoGenerateColumns = false;
                dgvEdit.AutoSize = true;
                dgvEdit.RowHeadersVisible = false;
                dgvEdit.DataSource = source;

                int columnPosition = 0;
                
                btnApprove.Enabled = true;

                switch (this.FieldContextData.PropertyType)
                {
                    case "String":
                        DataGridViewColumn(dgvEdit, "Required", new DataGridViewTextBoxColumn(), ++columnPosition);
                        DataGridViewColumn(dgvEdit, "Name", new DataGridViewTextBoxColumn(), ++columnPosition);
                        DataGridViewColumn(dgvEdit, "Value", new DataGridViewTextBoxColumn(), ++columnPosition);
                        DataGridViewColumn(dgvEdit, "Type", new DataGridViewTextBoxColumn(), ++columnPosition);
                        DataGridViewColumn(dgvEdit, "MappedPropertyInfo", new DataGridViewTextBoxColumn(), ++columnPosition);

                        source.Add(new ContextItem
                        {
                            //Required = this.FieldContextData.MappedPropertyInfo.CustomAttribute<IMappedPropertyAttribute>().Required,
                            Required = this.FieldContextData.IsFieldRequired,
                            Type = this.FieldContextData.PropertyType,
                            MappedPropertyInfo = FieldContextData.MappedPropertyInfo,
                            Name = FieldContextData.MappedPropertyAttributeInstanceName,
                            Value = FieldContextData.MappedPropertyInfo?.GetValue(this.TextractClaimImage.Claim) as string
                        });
                        break;

                    case "Field":
                        if (this.FieldContextData.MappedPropertAttribute.Name == "GroupFieldAttribute")
                        {
                            var type = FieldContextData.MappedGroupPropertyInfo?.PropertyType;
                            if (type != null)
                            {
                                DataGridViewColumn(dgvEdit, "Required", new DataGridViewTextBoxColumn(), ++columnPosition);
                                DataGridViewColumn(dgvEdit, "Name", new DataGridViewTextBoxColumn(), ++columnPosition);
                                dgvEdit.Columns.Add(CreateComboBoxWithEnums(type, ++columnPosition));
                                DataGridViewColumn(dgvEdit, "Type", new DataGridViewTextBoxColumn(), ++columnPosition);
                                DataGridViewColumn(dgvEdit, "MappedPropertyInfo", new DataGridViewTextBoxColumn(), ++columnPosition);

                                source.Add(new ContextItem
                                {
                                    //Required = this.FieldContextData.MappedGroupPropertyInfo.CustomAttribute<IMappedPropertyAttribute>().Required,
                                    Required = this.FieldContextData.IsFieldRequired,
                                    Type = this.FieldContextData.MappedGroupPropertyInfo.PropertyType.FullName,
                                    MappedPropertyInfo = this.FieldContextData.MappedGroupPropertyInfo,
                                    Name = FieldContextData.MappedPropertyGroupName,
                                    Value = FieldContextData.MappedGroupPropertyInfo?.GetValue(this.TextractClaimImage.Claim)
                                });
                            }
                        }
                        else
                        {
                            DataGridViewColumn(dgvEdit, "Required", new DataGridViewTextBoxColumn(), ++columnPosition);
                            DataGridViewColumn(dgvEdit, "Name", new DataGridViewTextBoxColumn(), ++columnPosition);
                            DataGridViewColumn(dgvEdit, "Value", new DataGridViewTextBoxColumn(), ++columnPosition);
                            DataGridViewColumn(dgvEdit, "Type", new DataGridViewTextBoxColumn(), ++columnPosition);
                            DataGridViewColumn(dgvEdit, "MappedPropertyInfo", new DataGridViewTextBoxColumn(), ++columnPosition);

                            //this.FieldContextData.ComositeValueProperties
                            this.FieldContextData.CompositeValuePropertyItems
                                .ToList().ForEach(p =>
                                    source.Add(new ContextItem
                                    {
                                        Required = p.IsRequired,
                                        Type = this.FieldContextData.PropertyType,
                                        MappedPropertyInfo = p.PropertyInfo,
                                        Name = p.PropertyInfo.Name,
                                        Value = p.PropertyInfo.GetValue(this.TextractClaimImage.Claim) as string
                                    }));
                        }
                        break;

                    case "Table":
                        tabControl.SelectTab("tabPage_EditTable");
                        break;

                    default:
                        if (this.FieldContextData.PropertyType.IsNotNullOrEmpty())
                            Logger.TraceWarning("Claim propery type: '{0}', does not yet support update.", this.FieldContextData.PropertyType);
                        dgvEdit.DataSource = null;
                        txtOCROriginalKey.Text = "";
                        txtOCROriginalValue.Text = "";
                        txtOCROriginalKeyConfidence.Text = "";
                        lblOCROriginalValueConfidence.Text = "";
                        changingTextboxConfidence = false;
                        UpdateStatusBar("OCR Field not mapped...", 3);
                        btnApprove.Enabled = false;
                        break;
                }

                if (source.Count > 0)
                {
                    tabControl.SelectTab("tabPage_EditField");

                    this.currentFieldTabOrder = FieldContextData.Element.TabOrder;
                    txtCurrentFieldNo.Text = this.currentFieldTabOrder.ToString();

                    if (highlightField)
                        this.currentFieldBoundingBox = FieldContextData.Element.Geometry?.BoundingBox;

                    DisplayOCRContextValues();

                    dgvEdit.AutoSize = true;
                    dgvEdit.Columns["Required"].Width = Convert.ToInt32(dgvEdit.Width * .1);
                    dgvEdit.Columns["Name"].Width = Convert.ToInt32(dgvEdit.Width * .38);
                    dgvEdit.Columns["Value"].Width = Convert.ToInt32(dgvEdit.Width * .5);
                    dgvEdit.Columns["Required"].ReadOnly = true;
                    dgvEdit.Columns["Name"].ReadOnly = true;
                    dgvEdit.Columns["Value"].ReadOnly = false;
                    dgvEdit.Columns["Type"].Visible = false;
                    dgvEdit.Columns["MappedPropertyInfo"].Visible = false;
                }
            }
        }

        void DisplayCoordinatesContext()
        {
            //if (FieldContextData.Elements?.Count() > 0)
            if (FieldContextData.Element != null)
            {

                string header = FieldContextData.Element.Key.HeaderText;
                string headerPrefix = header?.Substring(0, Math.Min(header.Length, 5));

                if (header.IsNullOrWhiteSpace())
                    return;

                IEnumerable<Field> fields = TextractClaimImage.Page
                    .Form.Fields.Where(f => f.Key.HeaderText != null && (
                            f.Key.HeaderText.Equals(header, StringComparison.OrdinalIgnoreCase)
                            || f.Key.HeaderText.StartsWith(headerPrefix))
                            );

                NamedCoordinates ncs = new NamedCoordinates();

                if (fields.Count() > 0)
                    namedCoordinatesRectangles.Clear();

                foreach (var field in fields)
                {
                    var n = new NamedCoordinate
                    {
                        ExactTextMatch = field.Key.Text,
                        FieldBoundingBox = field.Key.Geometry.BoundingBox,
                        ValueBoundingBox = field.Value.Geometry.BoundingBox,
                        IdealCenterKey = NewGeometry.IdealCenter(field.Key.Geometry.BoundingBox),
                        IdealCenterValue = NewGeometry.IdealCenter(field.Value.Geometry.BoundingBox),
                        HeaderText = field.Key.HeaderText ?? field.Key.Text,
                        GroupName = field.Key.GroupName ?? "GroupName",
                        Name = field.Key.Match ?? "Name",
                        Type = field.Key.Type ?? "string",
                        TabOrder = field.TabOrder,
                        MultiSelect = field.Key.MultiSelect.ToString()
                    };

                    ncs.Add(n);

                    namedCoordinatesRectangles.Add(getRectangleFromBoundingBox(n.FieldBoundingBox));
                    namedCoordinatesRectangles.Add(getRectangleFromBoundingBox(n.ValueBoundingBox));
                }

                //Todo: Get this from static
                Type[] AWSModelExtraTypes = new Type[] { typeof(NewGeometry), typeof(NewBoundingBox), typeof(MatchedFieldKey) };

                txtCoordinatesResults.Text = Serializer.SerializeToXML<List<NamedCoordinate>>(ncs, AWSModelExtraTypes);

                txtCoordinatesExactMatch.Text = ncs.First().ExactTextMatch;
                txtCoordinatesGroupName.Text = ncs.First().GroupName;
                txtCoordinatesHeaderText.Text = ncs.First().HeaderText;
                txtCoordinatesName.Text = ncs.First().Name;
            }
        }

        void DisplayOCRContext()
        {
            //string jsonData = null;
            //string elementName = null;

            //JsonSerializerSettings settings = new JsonSerializerSettings();
            //settings.ContractResolver = new DynamicContractResolver(new string[] { "Polygon" });

            ////var blocks = this.ZonedDocument.Page.GetBlocksAtCoordinate(new PointF { X = pos.left, Y = pos.top });

            //if (selectedElements.HasFlag(TextractElements.FieldKey) | selectedElements.HasFlag(TextractElements.FieldValue))
            //{
            //    elementName = TextractElements.Field.ToString();
            //    var elements = this.textractClaimImage.Page.GetFieldsByFieldValueCoordinate(coordinate);
            //    jsonData = JsonConvert.SerializeObject(elements, Formatting.Indented, settings);
            //}
            //else if (selectedElements.HasFlag(TextractElements.Line) )
            //{
            //    elementName = TextractElements.Line.ToString();
            //    var elements = this.textractClaimImage.Page.GetLinesByCoordinate(coordinate);
            //    jsonData = JsonConvert.SerializeObject(elements, Formatting.Indented, settings);
            //}

            //if (jsonData.IsNotNullOrWhiteSpace())
            if (FieldContextData.JSONdata.IsNotNullOrWhiteSpace())
            {
                ContextViewer viewer = new ContextViewer(this.TextractClaimImage.TextractDocument, FieldContextData.JSONdata, FieldContextData.ElementType);
                viewer.HighlightKeyWords = new List<HighlightKeyWord>
                        {
                                new HighlightKeyWord("\"HasMatch\": true", Color.Green, true ),
                                new HighlightKeyWord("\"Match\": null", Color.Black, false),
                                new HighlightKeyWord("\"Match\":", Color.Green, true),
                                new HighlightKeyWord("\"HasMatch\": false", Color.Red, false),
                                new HighlightKeyWord("\"Confidence\":", Color.Blue, true),
                                new HighlightKeyWord("\"Text\":", Color.Black, true)
                        };

                //viewer.HighlightKeyWords = new List<(string word, Color color, bool bold)> 
                //        {
                //                ("\"HasMatch\": true", Color.Green, true),
                //                ("\"Match\": null", Color.Black, false),
                //                ("\"Match\":", Color.Green, true),
                //                ("\"HasMatch\": false", Color.Red, false),
                //                ("\"Confidence\":", Color.Blue, true),
                //                ("\"Text\":", Color.Black, true)
                //        };

                //("\"HasMatch\": true", Color.LightGreen, true, true), "Match": null
                //            ("\"HasMatch\": false", Color.Yellow, false, false),
                //            ("\"Confidence\":", Color.Yellow, false, true),
                //            ("\"Text\":", Color.Yellow, false, true)
                viewer.Show(this);
            }
        }

        void DisplayClaimTableContext()
        {
            //List<TestTable>
            var rows = this.TextractClaimImage.Claim?.ServiceLines
                        .Where(r => (r.HasData & !r.IsHeader) | chkShowAllRows.Checked)
                        //.Select(r => new {r.HasData, r.LineItemNO, r.ServiceStartDate, r.ServiceEndDate, r.ChargedAmount, r.DiagnosisCodePointer1, r.ProcedureCode, r.ProcedureModifier1, r.ServiceUnitCount})
                        //.Select(r => new TestTable(r.ProcedureCode, r.ChargedAmount, r.ServiceUnitCount, r.LineNoteText)).ToList()
                        ;

            if (rows?.Count() < 1)
                return;

            BindingSource source = new BindingSource();
            source.DataSource = rows;
            //dgvEditTable.DataSource = source;
            // =========

            dgvEditTable.Columns.Clear();
            dgvEditTable.Rows.Clear();
            dgvEditTable.DataSource = null;
            //dgvEditTable.Refresh();

            dgvEditTable.AutoGenerateColumns = false;
            dgvEditTable.AutoSize = false;
            dgvEditTable.RowHeadersVisible = false;
            dgvEditTable.DataSource = source;

            int index = 0;
            
            //DataGridViewColumn(dgvEditTable, "RowId", new DataGridViewTextBoxColumn(), index++);
            DataGridViewColumn(dgvEditTable, "LineItemNO", "No", new DataGridViewTextBoxColumn(), index++);

            DataGridViewColumn(dgvEditTable, "HasData", "Include", new DataGridViewCheckBoxColumn(), index++);
            DataGridViewColumn(dgvEditTable, "ServiceStartDate", "Start Date", new DataGridViewTextBoxColumn(), index++, 200, true);
            //DataGridViewColumn(dgvEditTable, "ServiceEndDate", "End Date", new DataGridViewTextBoxColumn(), index++, 200);
            DataGridViewColumn(dgvEditTable, "OralCavityArea", "Area", new DataGridViewTextBoxColumn(), index++);
            DataGridViewColumn(dgvEditTable, "ToothCode", "Tooth Code", new DataGridViewTextBoxColumn(), index++);
            DataGridViewColumn(dgvEditTable, "ToothSurfaceCode1", new DataGridViewTextBoxColumn(), index++);
            DataGridViewColumn(dgvEditTable, "ProcedureCode", "Procedure Code", new DataGridViewTextBoxColumn(), index++, 100, true);
            DataGridViewColumn(dgvEditTable, "ServiceUnitCount", "Units", new DataGridViewTextBoxColumn(), index++);
            DataGridViewColumn(dgvEditTable, "LineNoteText", "Description", new DataGridViewTextBoxColumn(), index++, 400);
            DataGridViewColumn(dgvEditTable, "ChargedAmount", "Charged Amt", new DataGridViewTextBoxColumn(), index++, 0, true);

            if (chkShowAllRows.Checked)
            {
                DataGridViewColumn(dgvEditTable, "ProcedureModifier1", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "ProcedureModifier2", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "ProcedureModifier3", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "ProcedureModifier4", new DataGridViewTextBoxColumn(), index++);

                DataGridViewColumn(dgvEditTable, "ToothSurfaces", new DataGridViewTextBoxColumn(), index++);

                DataGridViewColumn(dgvEditTable, "ToothSurfaceCode1", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "ToothSurfaceCode2", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "ToothSurfaceCode3", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "ToothSurfaceCode4", new DataGridViewTextBoxColumn(), index++);

                DataGridViewColumn(dgvEditTable, "CavityDesignationCodes", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "CavityDesignationCode1", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "CavityDesignationCode2", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "CavityDesignationCode3", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "CavityDesignationCode4", new DataGridViewTextBoxColumn(), index++);

                DataGridViewColumn(dgvEditTable, "DiagnosisCodePointers", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "DiagnosisCodePointer1", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "DiagnosisCodePointer2", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "DiagnosisCodePointer3", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "DiagnosisCodePointer4", new DataGridViewTextBoxColumn(), index++);

                DataGridViewColumn(dgvEditTable, "UnitsMeasurementCode", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "ProductIDQualifier", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "PlaceOfService", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "ProcedureDate", new DataGridViewTextBoxColumn(), index++);

                DataGridViewColumn(dgvEditTable, "RowId", new DataGridViewTextBoxColumn(), index++);
                DataGridViewColumn(dgvEditTable, "NoteReferenceCode", new DataGridViewTextBoxColumn(), index++);
            }
        }

        void DataGridViewColumn(DataGridView dgv, string columnName, string headerText, DataGridViewColumn columnType, int displayIndex = -1, int width = 0, bool required = false)
        {
            if (displayIndex != -1)
                columnType.DisplayIndex = displayIndex;

            columnType.DataPropertyName = columnName;
            columnType.HeaderText = headerText.IsNullOrWhiteSpace() ? columnName : headerText;
            columnType.Name = columnName;


            int i = dgv.Columns.Add(columnType);
            if (width != 0)
            {
                dgv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgv.Columns[i].Width = width;
            }

            if (required)
            {
                dgv.EnableHeadersVisualStyles = false;
                dgv.Columns[i].HeaderCell.Style.BackColor = Color.LightSlateGray;
                dgv.Columns[i].HeaderCell.Style.ForeColor = Color.White;

                dgv.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                //dgv.Columns[i].DefaultCellStyle.ForeColor = Color.White;
                //dgv.Columns[i].DefaultCellStyle.BackColor = Color.LightSlateGray;
            }
        }

        void DataGridViewColumn(DataGridView dgv, string columnName, DataGridViewColumn columnType, int displayIndex = -1, int width = 0, bool required = false)
        {
            DataGridViewColumn(dgv, columnName, null, columnType, displayIndex, width, required);
        }

        void dgvEdit_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int? rowIdx = e?.RowIndex;
            int? colIdx = e?.ColumnIndex;

            if (rowIdx.HasValue && colIdx.HasValue)
            {
                var dgv = (DataGridView)sender;
                var valueCell = dgv?.Rows?[rowIdx.Value]?.Cells?["Value"]?.Value;
                var propertyInfoCell = dgv?.Rows?[rowIdx.Value]?.Cells?["MappedPropertyInfo"]?.Value as System.Reflection.PropertyInfo;

                if (valueCell == DBNull.Value) valueCell = null;

                if (propertyInfoCell != null)
                {
                    if (propertyInfoCell.PropertyType.IsEnum)
                        propertyInfoCell.SetValue(this.TextractClaimImage.Claim, Enum.Parse(propertyInfoCell.PropertyType, valueCell.ToString()));

                    if (propertyInfoCell.PropertyType == typeof(string))
                        propertyInfoCell.SetValue(this.TextractClaimImage.Claim, valueCell);

                    this.FieldContextData.SetUpdated(true);
                    this.imageBox.Invalidate();
                    UpdateStatusBar("Updated claim property...", 3);
                }
            };
        }

        void dgvEdit_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            var dgv = (DataGridView)sender;
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid.")
            {
                object value = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dgv.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    // Ignore DataGridView ComboBox error
                    e.ThrowException = false;
                }
            }
        }

        void dgvEditTable_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int? rowIdx = e?.RowIndex;
            int? colIdx = e?.ColumnIndex;

            if (rowIdx.HasValue && colIdx.HasValue)
            {
                var dgv = (DataGridView)sender;
                var valueCell = dgv?.Rows?[rowIdx.Value]?.Cells?[colIdx.Value]?.Value;
            }
        }

        void dgvValidation_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int? rowIdx = e?.RowIndex;
            int? colIdx = e?.ColumnIndex;

            if (rowIdx.HasValue && colIdx.HasValue)
            {
                var dgv = (DataGridView)sender;
                var valueCell = dgv?.Rows?[rowIdx.Value]?.Cells?["Value"]?.Value;
                var propertyInfoCell = dgv?.Rows?[rowIdx.Value]?.Cells?["MappedPropertyInfo"]?.Value as System.Reflection.PropertyInfo;

                if (valueCell == DBNull.Value) valueCell = null;

                if (propertyInfoCell != null)
                {
                    if (propertyInfoCell.PropertyType.IsEnum)
                        propertyInfoCell.SetValue(this.TextractClaimImage.Claim, Enum.Parse(propertyInfoCell.PropertyType, valueCell.ToString()));

                    if (propertyInfoCell.PropertyType == typeof(string))
                        propertyInfoCell.SetValue(this.TextractClaimImage.Claim, valueCell);

                    this.FieldContextData.SetUpdated(true);
                    //this.imageBox.Invalidate();
                    //UpdateStatusBar("Updated claim property...", 3);
                }
            };
        }

        void dgvValidation_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            var dgv = (DataGridView)sender;
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid.")
            {
                object value = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dgv.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    // Ignore DataGridView ComboBox error
                    e.ThrowException = false;
                }
            }
        }

        void ShowContextPanel(bool show = true)
        {
            if (mainSplitContainer.Panel2Collapsed)
            {
                mainSplitContainer.Panel2Collapsed = false;
                mainSplitContainer.SplitterDistance = this.Height - TextractClaimImageStyle.InfoPanelInitialHeight;
                //mainSplitContainer.Panel2MinSize = TextractClaimImageStyle.InfoPanelInitialHeight;

                //mainSplitContainer.Panel2.Height = TextractClaimImageStyle.InfoPanelInitialHeight;
            }
            else if (!show)
            {
                mainSplitContainer.Panel2Collapsed = true;
            }
        }

        void UpdateDrawingContextNamedCoordinate(int index = 1)
        {
            Type[] AWSModelExtraTypes = new Type[] { typeof(NewGeometry), typeof(NewBoundingBox), typeof(MatchedFieldKey) };
            List<NamedCoordinate> ncs = Serializer.DeseralizeFromXml<List<NamedCoordinate>>(txtCoordinatesResults.Text, AWSModelExtraTypes);

            if (ncs.Count >= index)
            {
                NamedCoordinate nc = ncs.ToArray()[index - 1];

                nc.ExactTextMatch = txtCoordinatesExactMatch.Text;
                nc.GroupName = txtCoordinatesGroupName.Text;
                nc.HeaderText = txtCoordinatesHeaderText.Text;
                nc.Name = txtCoordinatesName.Text;
                nc.Type = chkCoordinatesIsBoolean.Checked ? "bool" : "string";

                txtCoordinatesResults.Text = Serializer.SerializeToXML<List<NamedCoordinate>>(ncs, AWSModelExtraTypes);
            }
        }
        #endregion

        #region TextractViewer Methods

        void AdornNamedCoordinats(Graphics graphics)
        {
            this.TextractClaimImage.TextractDocument.NamedCoordinates
                .ForEach(c =>
                {
                    HighlightBoundingCircle(graphics, c?.IdealCenterValue, Color.HotPink, 5, 100, true);
                });
        }

        void EmphasizeBlocks(Graphics graphics, TextractElements selectedElemets)
        {

            this.TextractClaimImage.Cache.TextractDocument
                .ResponsePages.SelectMany(p => p.Blocks)
                .ToList().ForEach(b =>
                {
                    if (selectedElemets.HasFlag(TextractElements.Page)
                        && b.BlockType == "PAGE")
                    {
                        HighlightBoundingBox(graphics, b.Geometry.BoundingBox, TextractClaimImageStyle.ocrPageColor);
                    }
                    if (b.BlockType == "KEY_VALUE_SET")
                    {
                        if (selectedElemets.HasFlag(TextractElements.FieldKey)
                            && b.EntityTypes[0] == "KEY")
                        {
                            HighlightBoundingBox(graphics, b.Geometry.BoundingBox, TextractClaimImageStyle.ocrFieldKeyColor);
                        }
                        if (selectedElemets.HasFlag(TextractElements.FieldValue)
                            && b.EntityTypes[0] == "VALUE")
                        {
                            HighlightBoundingBox(graphics, b.Geometry.BoundingBox, TextractClaimImageStyle.ocrFieldValueColor);
                        }
                    }
                    if (selectedElemets.HasFlag(TextractElements.Line)
                        && b.BlockType == "LINE")
                    {
                        HighlightBoundingBox(graphics, b.Geometry.BoundingBox, TextractClaimImageStyle.ocrLineColor);
                    }

                }
            );
        }

        BoundingBox PaddedBoundingBox(BoundingBox bb, float paddingFactor)
        {
            if (bb == null)
                return null;

            PointF scaledPadding;

            //TextractClaimImageStyle.DefaultPadding.Width
            //if (ShowHighlights)
                scaledPadding = TextractClaimImage.ImagePosition(this.TextractClaimImage.SourceImage, new PointF(.000001F * paddingFactor, .0000005F * paddingFactor));
            //else
            //    scaledPadding = TextractClaimImage.ImagePosition(this.TextractClaimImage.SourceImage, new PointF(0, 0));

            return new BoundingBox { Top = bb.Top - scaledPadding.Y
                                    , Left = bb.Left - scaledPadding.X
                                    , Height = bb.Height + scaledPadding.Y
                                    , Width = bb.Width + scaledPadding.X };
        }

        Color FieldConfidenceHighlightColor(float actualConfidence, float minConfidence, bool approved = false)
        {
            Color retColor = Color.Black;

            float delta = actualConfidence - minConfidence;

            if (approved || delta >= TextractClaimImageStyle.ocrConfidenceBorderlineRange)
            {
                retColor = TextractClaimImageStyle.ocrConfidencePass;
            }
            else if (delta < TextractClaimImageStyle.ocrConfidenceBorderlineRange & delta >= 0)
            {
                retColor = TextractClaimImageStyle.ocrConfidenceBorderline;
            }
            else if (delta < 0)
            {
                retColor = TextractClaimImageStyle.ocrConfidenceFail;
            }

            return retColor;
        }


        void EmphasizeFields(Graphics graphics, TextractElements selectedElemets)
        {

            this.TextractClaimImage
                .Page?.Form.Fields
                .ForEach(f =>
                {
                    if (selectedElemets.HasFlag(TextractElements.Field))
                    {
                        var c = f.Match == null ? TextractClaimImageStyle.notMatchedFieldColor
                            : (f.MappedToClaim ? TextractClaimImageStyle.mappedFieldColor
                                                : TextractClaimImageStyle.matchedFieldColor);
                        HighlightBoundingBox(graphics, f.Geometry.BoundingBox, c, TextractClaimImageStyle.DefaultGauge, TextractClaimImageStyle.DefaultOpacity, f.UpdateToClaim);
                        //HighlightBoundingBox(graphics, f?.Geometry?.BoundingBox, TextractClaimImageStyle.matchedFieldColor);
                    }

                    if (selectedElemets.HasFlag(TextractElements.FieldKey))
                    {
                        HighlightBoundingBox(graphics, f.Key?.Geometry.BoundingBox, TextractClaimImageStyle.matchedFieldKeyColor);
                    }
                    if (selectedElemets.HasFlag(TextractElements.FieldValue))
                    {
                        var color = f.Match == null ? TextractClaimImageStyle.notMatchedFieldColor
                                : (f.MappedToClaim ? TextractClaimImageStyle.mappedFieldValueColor
                                : TextractClaimImageStyle.matchedFieldValueColor);

                        float paddingFactor = TextractClaimImageStyle.DefaultPaddingFactor;
                        float gauge = TextractClaimImageStyle.DefaultGauge;
                        bool fill = f.UpdateToClaim;

                        if (f.Required)
                        {
                            paddingFactor = TextractClaimImageStyle.LargePaddingFactor;
                            gauge = TextractClaimImageStyle.LargeGauge;
                            color = FieldConfidenceHighlightColor(f.Key.Confidence, f.MinConfidence, f.UpdateToClaim);
                            //fill = color != TextractClaimImageStyle.ocrConfidencePass;
                        }

                        HighlightBoundingBox(graphics,
                            PaddedBoundingBox(f.Value?.Geometry.BoundingBox, paddingFactor),
                            color,
                            gauge, 
                            TextractClaimImageStyle.DefaultOpacity, 
                            fill);
                    }
                });

            if (selectedElemets.HasFlag(TextractElements.Page))
            {
                //ToDo: decide if we need, not serialised currently
                //HighlightBoundingBox(graphics, this.TextractClaimImage.Page?.Geometry.BoundingBox, Color.Orange);
            }

            //if (selectedElemets.HasFlag(TextractElements.Line))
            //{
            //    this.TextractClaimImage
            //        .Page?.Lines
            //        .ForEach(l =>
            //        {
            //            var c = l?.Match == null ? Color.Red : Color.Blue;
            //            HighlightBoundingBox(graphics, l?.Geometry.BoundingBox, c);
            //        });
            //}

            if (selectedElemets.HasFlag(TextractElements.Table))
            {
                this.TextractClaimImage
                    .Page?.Tables?[0]
                    .Rows.ForEach(r => r.Cells
                        .Where(c => c.MappedToClaim).ToList()
                        .ForEach(c =>
                        {
                            HighlightBoundingBox(graphics, c?.Geometry.BoundingBox, c.MappedToClaim ? TextractClaimImageStyle.mappedCellColor : TextractClaimImageStyle.matchedCellColor);
                        })
                    );

                HighlightBoundingBox(graphics, this.TextractClaimImage
                    .Page?.Tables?[0].Geometry.BoundingBox, Color.Pink);
            }
        }

        void EmphasizeNamedCoordinates(Graphics graphics)
        {
            this.TextractClaimImage.TextractDocument.NamedCoordinates?
                .ForEach(c =>
                {
                    HighlightBoundingBox(graphics, c?.ValueBoundingBox, Color.HotPink);
                });
        }

        void HighlightBoundingCircle(Graphics graphics, PointF? center, Color color, float radius = TextractClaimImageStyle.DefaultGauge, int opacity = TextractClaimImageStyle.DefaultOpacity, bool fill = false)
        {
            if (center == null) return;

            PointF imagePosition = this.TextractClaimImage.ImagePosition(center.Value);
            PointF offsetPosition = imageBox.GetOffsetPoint(imagePosition);
            SizeF scaledRadius = imageBox.GetScaledSize(new SizeF(radius, 0));

            if (fill)
            {
                var c = Color.FromArgb((int)Math.Round(255.0 * (opacity / 100.0)), color.R, color.G, color.B);

                using (Brush b = new SolidBrush(c))
                {
                    graphics.FillCircle(b, offsetPosition.X, offsetPosition.Y, scaledRadius.Width);
                }
            }
            else
                using (Pen pen = new Pen(color))
                {
                    graphics.DrawCircle(pen, offsetPosition.X, offsetPosition.Y, scaledRadius.Width);
                }

            //Logger.TraceVerbose("Ideal ({0}, {1})\tAdjusted ({2}, {3})", center.Value.X, center.Value.Y, adjustedPosition.X, adjustedPosition.Y);
        }

        void HighlightBoundingBox(Graphics graphics, Model.BoundingBox box, Color color
            , float gauge = TextractClaimImageStyle.DefaultGauge, int opacity = TextractClaimImageStyle.DefaultOpacity, bool fill = false)
        {
            if (box == null) return;

            Model.BoundingBox boundingBox = this.TextractClaimImage.ImagePosition(box);
            RectangleF offsetPosition = imageBox.GetOffsetRectangle(boundingBox.Left, boundingBox.Top, boundingBox.Width, boundingBox.Height);
            SizeF scaledGauge = imageBox.GetScaledSize(new SizeF(gauge, 0));

            if (fill)
            {
                var c = Color.FromArgb((int)Math.Round(255.0 * (opacity / 100.0)), color.R, color.G, color.B);

                using (SolidBrush brush = new SolidBrush(c))
                {
                    graphics.FillRectangle(brush, offsetPosition.Left, offsetPosition.Top, offsetPosition.Width, offsetPosition.Height);
                }
            }
            else
                using (Pen pen = new Pen(color, scaledGauge.Width))
                {
                    graphics.DrawRectangle(pen, offsetPosition.Left, offsetPosition.Top, offsetPosition.Width, offsetPosition.Height);
                }
        }

        GraphicsHandle GetSelectedRectangleIndex(object sender, MouseEventArgs e)
        {
            if (fieldRectangles == null)
            {
                fieldRectangles = new List<RectangleF>();

                this.TextractClaimImage
                    .Page?.Form.Fields
                    .ForEach(f => fieldRectangles.Add(TextractClaimImage.BoundingBoxToRectangle(f.Value?.Geometry.BoundingBox)));
            }

            PointF location = imageBox.PointToImage(e.Location);

            PointF identPos = TextractClaimImage
                .ImageIdentityPosition(this.TextractClaimImage.SourceImage, new PointF { X = location.X, Y = location.Y });

            int i = -1;
            foreach (var r in fieldRectangles)
            {
                i++;
                if (r.Contains(e.Location))
                    return new GraphicsHandle(i, PointF.Subtract(identPos, new SizeF(r.Width, r.Height)), true);
            }

            return new GraphicsHandle();
        }

        #endregion

        #region ImageBox Methods

        private void imageBox_Click(object sender, EventArgs e)
        {
            ShowContextPanel();

            MouseEventArgs me = (MouseEventArgs)e;

            PointF imageClickLocation = TranslateMouseLocation(me);

            //var start = imageClickLocation;
            //var end = absoluteCenter;

            //if (TestLines == null) TestLines = new List<tempLine>();
            //TestLines.Add(new tempLine { Start = start, End = end, Color = Color.Green });

            //TestLines.Add(new tempLine { Start = rotatedimageClickLocation, End = absoluteCenter, Color = Color.Blue });

            //string tst = "";
            //tst = $"me.Location: ({me.Location.X}, {me.Location.Y})  imageclickloc: ({imageClickLocation.X}, {imageClickLocation.Y})" + Environment.NewLine;

            //textBox2.Text = "";
            //textBox2.Text = tst + textBox2.Text;

            PointF identityPosition = TextractClaimImage.ImageIdentityPosition(this.TextractClaimImage.SourceImage, imageClickLocation);

            if (me.Button == MouseButtons.Right)
            {
                this.FieldContextData = ContextData.GetContext(this.TextractClaimImage, identityPosition, this.SelectedTextractElements);
                DisplayOCRContext();
            }

            if (me.Button == MouseButtons.Left)
            {
                this.FieldContextData = ContextData.GetContext(this.TextractClaimImage, identityPosition, this.SelectedTextractElements);
                if (editMode == EditMode.EditCoordinate)
                    DisplayCoordinatesContext();

                if (editMode == EditMode.EditField)
                    DisplayClaimFieldContext();
            }
        }
        
        private void imageBox_DoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;

            PointF imageClickLocation = TranslateMouseLocation(me);

            if (me.Button == MouseButtons.Left)
            {
                PointF identityPosition = TextractClaimImage.ImageIdentityPosition(this.TextractClaimImage.SourceImage, imageClickLocation);
                //PointF identPos = TextractClaimImage.ImageIdentityPosition(this.TextractClaimImage.SourceImage, new PointF { X = location.X, Y = location.Y });

                this.FieldContextData = ContextData.GetContext(this.TextractClaimImage, new PointF { X = identityPosition.X, Y = identityPosition.Y }, this.SelectedTextractElements);
                
                DisplayClaimFieldContext();

                //if (editMode == EditMode.EditCoordinates)
                //    DisplayCoordinatesContext();

                //if (editMode == EditMode.EditField)
                //    DisplayClaimFieldContext();

            }
        }

        private void imageBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (imageBox.IsPointInImage(e.Location))
            {
                //// add a new landmark
                //this.AddLandmark(imageBox.PointToImage(e.Location));

                // force the image to repaint
                imageBox.Invalidate();
            }
        }

        private void imageBox_MouseDown(object sender, MouseEventArgs e)
        {

            if (editMode == EditMode.Drawing)
            {
                drawingCurrentPos = drawingStartPos = imageBox.PointToImage(e.Location, true);

                drawing = true;

                //// move rect code
                //isMouseDown = true;
                //selectedHandle = GetSelectedRectangleIndex(sender, e);
            }
        }

        private void imageBox_MouseLeave(object sender, EventArgs e)
        {
            positionToolStripStatusLabel.Text = string.Empty;
        }

        private void imageBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (editMode == EditMode.Drawing)
            {
                drawingCurrentPos = imageBox.PointToImage(e.Location, true);
                if (drawing)
                    imageBox.Invalidate();
            }

            this.UpdateCursorPosition(e.Location);
        }

        private void imageBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (editMode == EditMode.Drawing && drawing)
            {
                drawing = false;

                var rc = getRectangle();
                if (rc.Width > 0 && rc.Height > 0)
                    handDrawnRectangles.Add(rc);

                RectangleF irc = getIdentityRectangle(rc);

                string hXML = $"<Height>{irc.Height}</Height>";
                string lXML = $"<Left>{irc.Left}</Left>";
                string tXML = $"<Top>{irc.Top}</Top>";
                string wXML = $"<Width>{irc.Width}</Width>";
                string xCenterXML = $"<X>{irc.Left + irc.Width/2}</X>";
                string yCenterXML = $"<Y>{irc.Top + irc.Height/2}</Y>";

                string valueBB = $"<ValueBoundingBox>{hXML}{lXML}{tXML}{wXML}</ValueBoundingBox>";
                string idealCent = $"<IdealCenterValue>{xCenterXML}{yCenterXML}</IdealCenterValue>";

                txtDrawingResults.Text
                                        = FormatRectangle(rc) + Environment.NewLine
                    + FormatRectangleF(getIdentityRectangle(rc)) + Environment.NewLine
                    + FormatRectangleFCenter(getIdentityRectangle(rc)) + Environment.NewLine
                    + valueBB + Environment.NewLine
                    + idealCent + Environment.NewLine
                    + txtDrawingResults.Text + Environment.NewLine
                    ;


                //= FormatRectangle(rc) + Environment.NewLine
                //+ FormatRectangleF(getIdentityRectangle(rc)) + Environment.NewLine
                //+ FormatRectangleFCenter(getIdentityRectangle(rc)) + Environment.NewLine
                //+ txtDrawingResults.Text + Environment.NewLine
                //;

                ////MouseEventArgs me = (MouseEventArgs)e;

                ////PointF imageClickLocation = TranslateMouseLocation(e);
                //PointF imageStartLocation = new PointF { X = rc.X, Y = rc.Y };
                //PointF identityPosition = TextractClaimImage.ImageIdentityPosition(this.TextractClaimImage.SourceImage, imageStartLocation);
                //Field field = this.TextractClaimImage.Page?.Form.GetFieldByFieldCoordinate(identityPosition);

                //if (field == null) return;

                //BoundingBox bb = field.Key.Geometry.BoundingBox;

                //PointF identityPointTopLeft = new PointF { X = bb.Left, Y = bb.Top + bb.Height };

                //RectangleF valueBox = getIdentityRectangle(rc);
                //PointF identityBottomRight = new PointF { X = valueBox.Left + valueBox.Width, Y = valueBox.Top + valueBox.Height };


                //float dx = identityPointTopLeft.X;
                //float dy = identityPointTopLeft.Y;
                //float h = identityBottomRight.Y - identityPointTopLeft.Y;
                //float w = valueBox.Width;

                //string dxXML = $"<Top>{dx}</Top>";
                //string dyXML = $"<Left>{dy}</Left>";
                //string hXML = $"<Height>{h}</Height>";
                //string wXML = $"<Width>{w}</Width>";

                //string s = $"<ValueOffset>{hXML}{dyXML}{dxXML}{wXML}</ValueOffset>";

                //txtDrawingResults.Text = s + Environment.NewLine + txtDrawingResults.Text;

                ////txtDrawingResults.Text
                ////    = txtDrawingResults.Text + Environment.NewLine
                ////    + FormatRectangle(rc) + Environment.NewLine
                ////    + FormatRectangleF(getIdentityRectangle(rc)) + Environment.NewLine
                ////    + FormatRectangleFCenter(getIdentityRectangle(rc)) + Environment.NewLine
                ////    ;

                imageBox.Invalidate();
            }
        }

        private void imageBox_Paint(object sender, PaintEventArgs e)
        {
            
            btnCopyInsuredToPatient.Text = this.TextractClaimImage.Claim.PatientIsSetToInsured ? "Is Self" : "&Make Self";
            btnValidation_CopyInsuredToPatient.Text = btnCopyInsuredToPatient.Text;

            // remove test grid
            if (handDrawnRectangles.Count > 0)
            {
                using (Pen pen = new Pen(Color.Pink, scaledGauge.Width))
                {
                    handDrawnRectangles.ForEach(r =>
                    {
                        var rect = imageBox.GetOffsetRectangle(r);
                        e.Graphics.DrawRectangle(pen, rect);
                    });
                }
            }
            

            if (!ShowHighlights)
                return;

            // Display graphics text information
            foreach(var gtd in graphicsTextDictionary)
            {
                using (Font font = new Font(TextractClaimImageStyle.DefaultFont.Name, scaledFontSize.Width))
                {
                    PointF imagePoint = imageBox.GetOffsetPoint(TextractClaimImage.ImagePosition(gtd.Value.Location));
                    e.Graphics.DrawString(gtd.Value.Text, font, gtd.Value.Brush, imagePoint);
                }
            }


            //SizeF scaledGauge = imageBox.GetScaledSize(new SizeF(TextractClaimImageStyle.DefaultGauge, 0));

            if (this.imageRotationDegrees != 0)
                e.Graphics.Transform = TextractClaimImage.RotationMatrix(this.imageRotationDegrees, this.ImageRelativeCenter);

            if (this.TextractClaimImage != null)
            {
                //if (this.HighlightClaimsData == HighlightMode.Matched)
                //    EmphasizeFields(e.Graphics, this.SelectedTextractElements);
                //else if (this.HighlightClaimsData == HighlightMode.OCR)
                //    EmphasizeBlocks(e.Graphics, this.SelectedTextractElements);

                if (this.HighlightMatchedClaimsData)
                    EmphasizeFields(e.Graphics, this.SelectedTextractElements);
                else
                    EmphasizeBlocks(e.Graphics, this.SelectedTextractElements);

                //PaintAdornments
                if (this.SelectedTextractViewerAdornments.HasFlag(TextractViewerAdornments.CoordinateBox))
                    EmphasizeNamedCoordinates(e.Graphics);

                if (this.SelectedTextractViewerAdornments.HasFlag(TextractViewerAdornments.CoordinateCenter))
                    AdornNamedCoordinats(e.Graphics);

                //e.Graphics.ScaleTransform(2.0F, 2.0F);
            }

            ////Draw the point of rotation.
            //PointF relativeCenter = imageBox.GetOffsetPoint(TextractClaimImage.AbsoluteImageCenter(this.TextractClaimImage.SourceImage));
            //e.Graphics.FillEllipse(Brushes.Red, relativeCenter.X - scaledGauge.Width*10, relativeCenter.Y - scaledGauge.Width * 10, 20 * scaledGauge.Width, 20 * scaledGauge.Width);
            ////Debug.Print($"centerO x:{relativeCenter.X}, y:{relativeCenter.Y}");

            //if (TestLines?.Count > 0)
            //{
            //    using (Pen pen = new Pen(Color.Blue, scaledGauge.Width * 2))
            //    {
            //        TestLines.ForEach(r =>
            //        {
            //            pen.Color = r.Color;
            //            var oStart = imageBox.GetOffsetPoint(r.Start);
            //            var oEnd = imageBox.GetOffsetPoint(r.End);
            //            e.Graphics.DrawLine(pen, oStart, oEnd);
            //        });
            //    }
            //}

            if (namedCoordinatesRectangles.Count > 0)
            {
                using (Pen pen = new Pen(Color.Orange, scaledGauge.Width))
                {
                    namedCoordinatesRectangles.ForEach(r =>
                    {
                        var rect = imageBox.GetOffsetRectangle(r);
                        e.Graphics.DrawRectangle(pen, rect);
                    });
                }
            }

            if (this.currentFieldBoundingBox != null)
                HighlightBoundingBox(e.Graphics, this.currentFieldBoundingBox, Color.Orange, 3, 25, true);

            //if (namedCoordinatesRectangles.Count > 0)
            if (handDrawnRectangles.Count > 0)
            {
                using (Pen pen = new Pen(Color.Pink, scaledGauge.Width))
                {
                    handDrawnRectangles.ForEach(r =>
                    {
                        var rect = imageBox.GetOffsetRectangle(r);
                        e.Graphics.DrawRectangle(pen, rect);
                    });
                }
            }
            //Debug.Print($"getRectangle x:{getRectangle().X}, width:{getRectangle().Width}");
            //Debug.Print($"OffsetRectangle x:{imageBox.GetOffsetRectangle(getRectangle()).X}, width:{imageBox.GetOffsetRectangle(getRectangle()).Width}");

            if (drawing)
                e.Graphics.DrawRectangle(new Pen(Color.Red, scaledGauge.Width), imageBox.GetOffsetRectangle(getRectangle()));

        }

        private void imageBox_Resize(object sender, EventArgs e)
        {
            this.UpdateStatusBar();
        }

        private void imageBox_Scroll(object sender, ScrollEventArgs e)
        {
            this.UpdateStatusBar();
        }

        private void imageBox_ZoomChanged(object sender, EventArgs e)
        {
            this.scaledGauge = imageBox.GetScaledSize(new SizeF(TextractClaimImageStyle.DefaultGauge, 0));
            this.scaledFontSize = imageBox.GetScaledSize(new SizeF(TextractClaimImageStyle.DefaultFont.Size, 0));

            this.UpdateStatusBar();
        }

        private void imageBox_ZoomLevelsChanged(object sender, EventArgs e)
        {
            this.FillZoomLevelSelector();
        }

        #endregion

        #region Status Methods

        void UpdateCursorPosition(System.Drawing.Point location)
        {
            if (TextractClaimImage == null)
                return;

            System.Drawing.Point point = imageBox.PointToImage(location);
            PointF identity = TextractClaimImage.ImageIdentityPosition(TextractClaimImage.SourceImage, point);

            positionToolStripStatusLabel.Text = $"Identity Position: [{identity.X},{identity.Y}] Image: " + this.FormatPoint(point);
        }

        protected override void StatusMessage()
        {
            base.StatusMessage();
        }

        protected override void StatusMessage(string Message)
        {
            if (Message.IsNullOrEmpty())
                this.StatusToolStripStatusLabel.Text = "Ready...";
            else
                this.StatusToolStripStatusLabel.Text = Message;

            this.statusStrip.Refresh();
        }

        void UpdateStatusBar(string appStatus = null, int secondsToClear = 0)
        {
            ComboBox_ZoomLevel.Text = string.Format("{0}%", imageBox.Zoom);
            zoomToolStripStatusLabel.Text = string.Format("{0}%", imageBox.Zoom);
            //autoScrollPositionToolStripStatusLabel.Text = this.FormatPoint(imageBox.AutoScrollPosition);
            //imageSizeToolStripStatusLabel.Text = this.FormatRectangle(imageBox.GetImageViewPort());
            //tsslMousePosition.Text = $"Image: [{e.X},{e.Y}]";

            this.StatusMessage(appStatus, secondsToClear);
        }
        #endregion

        #region Graphics and Drawing Methods

        Rectangle getRectangle()
        {
            return new Rectangle(
                Math.Min(drawingStartPos.X, drawingCurrentPos.X),
                Math.Min(drawingStartPos.Y, drawingCurrentPos.Y),
                Math.Abs(drawingStartPos.X - drawingCurrentPos.X),
                Math.Abs(drawingStartPos.Y - drawingCurrentPos.Y));
        }

        RectangleF getIdentityRectangle(Rectangle rectangle)
        {
            PointF orign = TextractClaimImage.ImageIdentityPosition(this.TextractClaimImage.SourceImage, rectangle.Location);
            PointF offset = TextractClaimImage.ImageIdentityPosition(this.TextractClaimImage.SourceImage, new System.Drawing.Point(rectangle.Width, rectangle.Height));

            return new RectangleF(orign.X, orign.Y, offset.X, offset.Y);
        }

        Rectangle getRectangleFromBoundingBox(Model.BoundingBox boundingBox)
        {
            var imagePosition = TextractClaimImage.ImagePosition(boundingBox);
            return new Rectangle((int)imagePosition.Left, (int)imagePosition.Top, (int)imagePosition.Width, (int)imagePosition.Height);
        }

        void SetPatientToInsured()
        {
            if (this.TextractClaimImage.Claim.PatientIsSetToInsured)
            {
                this.TextractClaimImage.Claim.SetPatientToInsured();
                graphicsTextDictionary.Add(ADAClaim.InsuredIsThePatientLiteral, TextractClaimImageStyle.DefaultInsuredIsPatientNoticeLocation, new SolidBrush(TextractClaimImageStyle.mappedFieldColor));
            }
            else
            {
                if (graphicsTextDictionary.ContainsKey(ADAClaim.InsuredIsThePatientLiteral))
                    graphicsTextDictionary.Remove(ADAClaim.InsuredIsThePatientLiteral);
            }
        }

        PointF TranslateMouseLocation(MouseEventArgs me)
        {
            PointF imageClickLocation = imageBox.PointToImage(me.Location);
            PointF absoluteCenter = this.TextractClaimImage.AbsoluteImageCenter(this.TextractClaimImage.SourceImage);
            PointF rotatedimageClickLocation = TextractClaimImage.RotatePoint(imageClickLocation, absoluteCenter, this.imageRotationDegrees);

            if (this.imageRotationDegrees != 0)
                imageClickLocation = rotatedimageClickLocation;

            return imageClickLocation;
        }
        #endregion

        #region ToolStrip, Menus & Buttons

        // ToDo: standardize all these different form control names 
        private void txtCurrentFieldNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))// && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtCurrentFieldNo_Leave(object sender, EventArgs e)
        {
            currentFieldTabOrder = int.Parse(txtCurrentFieldNo.Text);
        }

        private void ComboBox_NamedCoordinates_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateStatusBar("Change Form Version...");
            ReloadImage(ComboBox_NamedCoordinates.Text);
            UpdateStatusBar();
        }

        private void ComboBox_ZoomLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            int zoom = Convert.ToInt32(ComboBox_ZoomLevel.Text.Substring(0, ComboBox_ZoomLevel.Text.Length - 1));
            imageBox.Zoom = zoom;
        }

        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            switch (e.TabPage.Tag)
            {
                case "Validation":
                    editMode = EditMode.Validation;
                    break;
                case "EditCoordinates":
                    editMode = EditMode.EditCoordinate;
                    break;
                case "EditField":
                    editMode = EditMode.EditField;
                    break;
                case "EditTable":
                    editMode = EditMode.EditTable;
                    break;
                case "Drawing":
                    editMode = EditMode.Drawing;
                    break;
                default:
                    break;
            }
        }

        private void tsButtons_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem tsItem = (ToolStripMenuItem)sender;

            Enum.TryParse(tsItem.Tag.ToString(), out TextractElements selectedElement);

            this.SelectedTextractElements = tsItem.Checked
                            ? this.SelectedTextractElements | selectedElement
                            : this.SelectedTextractElements & ~selectedElement;

            // force repaint
            this.Refresh();
        }

        private void tsButton_Coordinates_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem tsItem = (ToolStripMenuItem)sender;

            Enum.TryParse(tsItem.Tag.ToString(), out TextractViewerAdornments selectedElement);

            this.SelectedTextractViewerAdornments = tsItem.Checked
                            ? this.SelectedTextractViewerAdornments | selectedElement
                            : this.SelectedTextractViewerAdornments & ~selectedElement;

            // force repaint
            this.Refresh();

            //this.HighlightClaimsDataMatch = tsButton.Checked;

        }

        private void tsButton_Orientation_Click(object sender, EventArgs e)
        {
            ToolStripButton orientationButton = (ToolStripButton)sender;

            if (this.mainSplitContainer.Orientation == Orientation.Vertical)
            {
                this.mainSplitContainer.Orientation = Orientation.Horizontal;
                //orientationButton.Text = "&Horizontal";
                this.mainSplitContainer.SplitterDistance = 200; // MainPanelMinSizeHorozontal;
            }
            else
            {
                this.mainSplitContainer.Orientation = Orientation.Vertical;
                //orientationButton.Text = "&Vertical";
            }

        }

        private void btnCoordinates_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch (btn.Tag)
            {
                case "Clear":
                    txtCoordinatesResults.Text = string.Empty;
                    namedCoordinatesRectangles.Clear();
                    this.Refresh();
                    break;

                case "Copy":
                    Clipboard.SetText(txtCoordinatesResults.Text);
                    break;

                case "Update":
                    UpdateDrawingContextNamedCoordinate();
                    break;

                default:
                    break;
            }
        }

        private void btnDrawing_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch (btn.Tag)
            {
                case "Clear":
                    txtDrawingResults.Text = string.Empty;
                    handDrawnRectangles.Clear();
                    this.Refresh();
                    break;

                case "Copy":
                    if(txtDrawingResults.Text.IsNotNullOrWhiteSpace())
                        Clipboard.SetText(txtDrawingResults.Text);
                    break;

                //case "Update":
                //    UpdateDrawingContextNamedCoordinate();
                //    break;

                default:
                    break;
            }
        }

        private void btnFieldNavigation_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch (btn.Tag)
            {
                case "Previous":
                    currentFieldTabOrder--;
                    break;
                case "Next":
                    currentFieldTabOrder++;
                    break;
                default:
                    break;
            }

            try
            {
                var field = this.TextractClaimImage.Page.Form.Fields.FirstOrDefault(f => f.TabOrder == currentFieldTabOrder);
                
                if (field == null)
                {
                    var coords = this.TextractClaimImage.Cache.TextractDocument.NamedCoordinates.Where(n => n.TabOrder == currentFieldTabOrder);
                    var coord = coords.FirstOrDefault();
                    field = new Field(coord);
                }
                this.FieldContextData = ContextData.GetContext(this.TextractClaimImage, field, this.SelectedTextractElements);

                if (HighlightOnTab && field?.Geometry != null)
                {
                    PointF imagePosition = TextractClaimImage.ImagePosition(((NewGeometry)field.Geometry).Center);
                    if (this.imageRotationDegrees != 0)
                    {
                        PointF absoluteCenter = this.TextractClaimImage.AbsoluteImageCenter(this.TextractClaimImage.SourceImage);
                        imagePosition = TextractClaimImage.RotatePoint(imagePosition, absoluteCenter, this.imageRotationDegrees);
                    }

                    this.imageBox.CenterAt(imagePosition.X, imagePosition.Y);
                }

                DisplayClaimFieldContext(HighlightOnTab);
                this.imageBox.Invalidate();
            }
            catch (Exception ex)
            {
                Logger.TraceError(ex);
            }
        }

        private void chkGeneric_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            switch (chk.Tag)
            {
                case "Highlight":
                    this.imageBox.Invalidate();
                    break;
                case "ShowAllData":
                    DisplayClaimTableContext();
                    break;
                default:
                    break;
            }
        }

        private void tsButton_MatchedFields_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripButton tsButton = (ToolStripButton)sender;

            this.HighlightMatchedClaimsData = tsButton.Checked;

            this.Refresh();
        }

        private void tsSplitButtonRotation_ButtonClick(object sender, EventArgs e)
        {
            tsRotate_FlipImage_Click(sender, e);
        }
        
        private void tsRotate_FlipImage_Click(object sender, EventArgs e)
        {
            UpdateStatusBar("Rotating Textract Image...");
            imageRotationDegrees = imageRotationDegrees >= 180F ? 0 : 180F;
            this.TextractClaimImage.SourceImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
            this.ReloadImage(ComboBox_NamedCoordinates.Text);
            this.Refresh();
            UpdateStatusBar();
        }

        private void tsRotate_Rotate90_Click(object sender, EventArgs e)
        {
            // ToDo: Refactor with tsRotate_FlipImage_Click
            UpdateStatusBar("Rotating Textract Image...");
            imageRotationDegrees = imageRotationDegrees >= 270F ? 0 : imageRotationDegrees + 90F;
            this.TextractClaimImage.SourceImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            this.ReloadImage(ComboBox_NamedCoordinates.Text);
            this.Refresh();
            UpdateStatusBar();
        }

        private void ToolStripSplitButton_Highlights_ButtonClick(object sender, EventArgs e)
        {
            ShowHighlights = !ShowHighlights;

            Debug.Print($"show highlights: {ShowHighlights}");
            ToolStripSplitButton_Highlights.Text = ShowHighlights ? "Highlights On" : "Highlights Off";

            imageBox.Refresh();
        }

        private void highlightsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem tsMenu = (ToolStripMenuItem)sender;

            switch (tsMenu.Tag)
            {
                case "All":
                    matchedFieldsToolStripMenuItem.Checked = false;
                    break;
                case "Matched":
                    allOCRFieldsToolStripMenuItem.Checked = false;
                    break;
                default:
                    break;
            }

            this.HighlightMatchedClaimsData = tsMenu.Tag.ToString() == "Matched";

            this.Refresh();
        }

        private void tsButton_Test1_Click(object sender, EventArgs e)
        {
            //var rc = getRectangle();
            //if (rc.Width > 0 && rc.Height > 0)
            //    handDrawnRectangles.Add(rc);

            for (int i=0; i< 10; i++)
            {
                var rc = new BoundingBox { Height = 1, Left = i/10F, Top = 0, Width = .1F };
                if (rc.Width > 0 && rc.Height > 0)
                    handDrawnRectangles.Add(getRectangleFromBoundingBox(rc));

                var rr = new BoundingBox { Height = .1F, Left = 0, Top = i / 10F, Width = 1 };
                if (rr.Width > 0 && rr.Height > 0)
                    handDrawnRectangles.Add(getRectangleFromBoundingBox(rr));
            }
        }

        private void ViewerJpg_KeyDown(object sender, KeyEventArgs e)
        {
            //Debug.Print(e.KeyCode.ToString());
            if ((e.KeyCode == Keys.H) && e.Control)
            {
                ToolStripSplitButton_Highlights_ButtonClick(sender, e);
                e.Handled = true;
            }
        }

        private void btnTestClear_Click(object sender, EventArgs e)
        {
            TestLines.Clear();
            textBox2.Text = "";
        }

        private void tabPage_EditField_Click(object sender, EventArgs e)
        {

        }

        private void mainSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            
            //mainSplitContainer.Panel2Collapsed = false;
            //mainSplitContainer.SplitterDistance = this.Height - TextractClaimImageStyle.InfoPanelInitialHeight;
            txtDrawingResults.Text = $"({e.SplitX}, {e.SplitY}) ({e.X},{e.Y})" + Environment.NewLine + txtDrawingResults.Text;
        }

        private void ContextButtons_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            switch (b.Tag)
            {
                case "Approve":
                    this.FieldContextData.SetUpdated(true);
                    DisplayOCRContextValues();
                    this.imageBox.Invalidate();
                    UpdateStatusBar("Updated claim property...", 3);
                    break;

                case "ApproveValidation":
                    this.TextractClaimImage.Claim.ManualApproval = true;
                    bool f = this.TextractClaimImage.Claim.IsValid;
                    CreateValidationResultContext();

                    //this.FieldContextData.SetUpdated(true);
                    //DisplayOCRContextValues();
                    //this.imageBox.Invalidate();
                    //UpdateStatusBar("Updated claim property...", 3);
                    break;

                case "MakeSelf":
                    // ToDo: cleanu this up
                    if (this.TextractClaimImage.Claim.SetPatientToInsured())
                        SetPatientToInsured();
                    else
                        MessageBox.Show(this, "Cannot set patient to insured, verify that Insured Last Name and ID are valid and then try again.", "Warning");
                        
                    break;

                case "ResetPayee":
                    // reset payee assigned to patient back to provider
                    break;

                case "Hide":
                    ShowContextPanel(false);
                    //mainSplitContainer.Panel2Collapsed = true;
                    break;

                case "Orientation":
                    if (this.mainSplitContainer.Orientation == Orientation.Vertical)
                    {
                        this.mainSplitContainer.Orientation = Orientation.Horizontal;
                        //orientationButton.Text = "&Horizontal";
                        this.mainSplitContainer.SplitterDistance = 200; // MainPanelMinSizeHorozontal;
                    }
                    else
                    {
                        this.mainSplitContainer.Orientation = Orientation.Vertical;
                        //orientationButton.Text = "&Vertical";
                    }
                    break;

                case "Help":
                    string desc = "";
                    desc += "1) Make sure 'Relationship to Policyholder' is correct.  If 'Self' is selected, Subscriber values will be copied to Patient.";
                    desc += Environment.NewLine;
                    desc += "2) Business names found in Billing and Treating Provider are one line and entered in LastName field.";
                    MessageBox.Show(desc);
                    break;

                default:
                    break;
            }

            //this.imageBox.Invalidate();
            this.Refresh();
        }
        #endregion

        private void txtOCROriginalKeyConfidence_Leave(object sender, EventArgs e)
        {
            float previousMin = this.FieldContextData.Element.MinConfidence;

            if (previousMin == 0)
            {
                txtOCROriginalKeyConfidence.Text = $"{0}%";
                return;
            }

            float newMinimum;
            if (!float.TryParse(txtOCROriginalKeyConfidence.Text.Replace("%", ""), out newMinimum))
            {
                MessageBox.Show(this, $"Please enter a percentage value between 10% and 100%.", "Error");
                newMinimum = 0;
            }

            if (newMinimum > 0 
                && MessageBox.Show(this, $"Do you wish to change minimum confidence from {previousMin} to {txtOCROriginalKeyConfidence.Text} for coordinate: {this.FieldContextData.Element.Match}?", "Confirmation"
                , MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                NamedCoordinates.UpdateConfidence(this.TextractClaimImage.MetaData.FormType, this.FieldContextData.Element.Match, newMinimum);
                this.FieldContextData.SetMinimumConfidence(newMinimum);
                txtOCROriginalKeyConfidence.Text = $"{newMinimum}%";
            }
            else
                txtOCROriginalKeyConfidence.Text = $"{this.FieldContextData.Element.MinConfidence}%";

            DisplayOCRContextValues();
            changingTextboxConfidence = false;
        }
        bool changingTextboxConfidence;
        private void txtOCROriginalKeyConfidence_TextChanged(object sender, EventArgs e)
        {
            changingTextboxConfidence = true;
        }
    }
}
