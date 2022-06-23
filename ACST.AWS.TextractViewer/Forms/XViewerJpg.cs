
namespace ACST.AWS.TextractViewer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using ACST.AWS.Common;
    using Model = Amazon.Textract.Model;
    using TextractClaimMapper.ADA;

    using Cyotek.Windows.Forms;
    using ACST.AWS.Textract.Model;

    internal partial class ViewerJpg 
        : BaseForm
    {

        #region Fields & Properties

        //bool isMouseDown = false;

        //GraphicsHandle selectedHandle;

        List<RectangleF> fieldRectangles;

        enum EditMode
        {
            EditClaim,
            EditField,
            EditTable,
            EditCoordinates,
            Drawing
        }

        const float defaultGauge = 3;
        const int defaultOpacity = 25;

        EditMode editMode;
        bool cacheLoading;

        bool drawing;
        Point drawingStartPos;
        Point drawingCurrentPos;

        List<Rectangle> handDrawnRectangles = new List<Rectangle>();
        List<Rectangle> namedCoordinatesRectangles = new List<Rectangle>();
        Model.BoundingBox currentFieldBoundingBox;

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

        ContextData FieldContextData { get; set; }

        TextractElements SelectedTextractElements { get; set; }

        TextractViewerAdornments SelectedTextractViewerAdornments { get; set; }

        internal TextractClaimImage TextractClaimImage { get; set; }
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
            switch (keyData)
            {
                case Keys.Tab | Keys.Shift:
                    btnPrevious.PerformClick();
                    break;
                case Keys.Tab:
                    btnNext.PerformClick();
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
            ComboBox_NamedCoordinates.Width = 200; // DropDownWidth(ComboBox_NamedCoordinates) * 2;
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

            PopulateContextTableDataGridView();
        }

        void ReloadImage(string coordinatesId)
        {
            if (cacheLoading)
                return;
            
            var nc = new ADANamedCoordinates(namedCoordinatesFileNameDictionary[coordinatesId]);

            this.TextractClaimImage.Cache.ReBuildClaim(nc);

            this.Refresh();
        }

        void SetToolbarButtons()
        {
            toolStripButton_MatchedFields.Checked = HighlightMatchedClaimsData;

            ToolStripMenuItem_Page.Checked = SelectedTextractElements.HasFlag(TextractElements.Page);
            ToolStripMenuItem_Field.Checked = SelectedTextractElements.HasFlag(TextractElements.Field);
            ToolStripMenuItem_FieldKey.Checked = SelectedTextractElements.HasFlag(TextractElements.FieldKey);
            ToolStripMenuItem_FieldValue.Checked = SelectedTextractElements.HasFlag(TextractElements.FieldValue);
            ToolStripMenuItem_Table.Checked = SelectedTextractElements.HasFlag(TextractElements.Table);
        }
        #endregion

        #region ContextData Metohds

        DataGridViewComboBoxColumn CreateComboBoxWithEnums(Type type, int displayIndex = -1)
        {
            DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn();
            if (displayIndex != -1)
                combo.DisplayIndex = displayIndex;
            combo.DataSource = Enum.GetValues(type);
            combo.ValueType = type;
            combo.DataPropertyName = "Value";
            combo.Name = "Value";
            return combo;
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

        void PopulateContextTableDataGridView()
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
            DataGridViewColumn(dgvEditTable, "ServiceStartDate", "Start Date", new DataGridViewTextBoxColumn(), index++, 200);
            DataGridViewColumn(dgvEditTable, "ServiceEndDate", "End Date", new DataGridViewTextBoxColumn(), index++, 200);
            DataGridViewColumn(dgvEditTable, "OralCavityArea", "Area", new DataGridViewTextBoxColumn(), index++);
            DataGridViewColumn(dgvEditTable, "ToothCode", "Tooth Code", new DataGridViewTextBoxColumn(), index++);
            DataGridViewColumn(dgvEditTable, "ProcedureCode", "Procedure Code", new DataGridViewTextBoxColumn(), index++);
            DataGridViewColumn(dgvEditTable, "ServiceUnitCount", "Units", new DataGridViewTextBoxColumn(), index++);
            DataGridViewColumn(dgvEditTable, "LineNoteText", "Description", new DataGridViewTextBoxColumn(), index++, 400);
            DataGridViewColumn(dgvEditTable, "ChargedAmount", "Charged Amt", new DataGridViewTextBoxColumn(), index++);

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

        void DataGridViewColumn(DataGridView dgv, string columnName, string headerText, DataGridViewColumn columnType, int displayIndex = -1, int width = 0)
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
        }

        void DataGridViewColumn(DataGridView dgv, string columnName, DataGridViewColumn columnType, int displayIndex = -1, int width = 0)
        {
            DataGridViewColumn(dgv, columnName, null, columnType, displayIndex, width);
        }

        void DisplayClaimFieldContext(bool highlightField = false)
        {
            if (this.FieldContextData.ElementType == "Table" & this.FieldContextData.CellElements?.Count() > 0)
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

                switch (this.FieldContextData.PropertyType)
                {
                    case "String":
                        DataGridViewColumn(dgvEdit, "Name", new DataGridViewTextBoxColumn(), 1);
                        DataGridViewColumn(dgvEdit, "Value", new DataGridViewTextBoxColumn(), 2);
                        DataGridViewColumn(dgvEdit, "Type", new DataGridViewTextBoxColumn(), 3);
                        DataGridViewColumn(dgvEdit, "MappedPropertyInfo", new DataGridViewTextBoxColumn(), 4);

                        source.Add(new ContextItem
                        {
                            Type = this.FieldContextData.PropertyType,
                            MappedPropertyInfo = FieldContextData.MappedPropertyInfo,
                            Name = FieldContextData.MappedPropertyName,
                            Value = FieldContextData.MappedPropertyInfo?.GetValue(this.TextractClaimImage.Claim) as string
                        });
                        break;

                    case "Field":
                        if (this.FieldContextData.MappedPropertAttributeType.Name == "GroupFieldAttribute")
                        {
                            DataGridViewColumn(dgvEdit, "Name", new DataGridViewTextBoxColumn(), 1);
                            dgvEdit.Columns.Add(CreateComboBoxWithEnums(FieldContextData.MappedGroupPropertyInfo.PropertyType, 2));
                            DataGridViewColumn(dgvEdit, "Type", new DataGridViewTextBoxColumn(), 3);
                            DataGridViewColumn(dgvEdit, "MappedPropertyInfo", new DataGridViewTextBoxColumn(), 4);

                            source.Add(new ContextItem
                            {
                                Type = this.FieldContextData.MappedGroupPropertyInfo.PropertyType.FullName,
                                MappedPropertyInfo = this.FieldContextData.MappedGroupPropertyInfo,
                                Name = FieldContextData.MappedPropertyGroupName,
                                Value = FieldContextData.MappedGroupPropertyInfo?.GetValue(this.TextractClaimImage.Claim)
                            });
                        }
                        else
                        {
                            DataGridViewColumn(dgvEdit, "Name", new DataGridViewTextBoxColumn(), 1);
                            DataGridViewColumn(dgvEdit, "Value", new DataGridViewTextBoxColumn(), 2);
                            DataGridViewColumn(dgvEdit, "Type", new DataGridViewTextBoxColumn(), 3);
                            DataGridViewColumn(dgvEdit, "MappedPropertyInfo", new DataGridViewTextBoxColumn(), 4);

                            this.FieldContextData.ComositeValueProperties
                                .ToList().ForEach(p =>
                                    source.Add(new ContextItem
                                    {
                                        Type = this.FieldContextData.PropertyType,
                                        MappedPropertyInfo = p,
                                        Name = p.Name,
                                        Value = p.GetValue(this.TextractClaimImage.Claim) as string
                                    }));
                        }
                        break;

                    case "Table":
                        tabControl.SelectTab("tabPage_EditTable");
                        break;

                    default:
                        Logger.TraceWarning("Claim propery type: '{0}', does not yet support update.", this.FieldContextData.PropertyType);
                        dgvEdit.DataSource = null;
                        txtOCROriginalKey.Text = "";
                        txtOCROriginalValue.Text = "";
                        UpdateStatusBar("OCR Field not mapped...", 3);
                        break;
                }

                if (source.Count > 0)
                {
                    tabControl.SelectTab("tabPage_EditField");

                    this.currentFieldTabOrder = FieldContextData.Element.TabOrder;
                    txtCurrentFieldNo.Text = this.currentFieldTabOrder.ToString();

                    if (highlightField)
                        this.currentFieldBoundingBox = FieldContextData.Element.Geometry?.BoundingBox;

                    if (mainSplitContainer.Orientation == Orientation.Vertical)
                        txtOCROriginalKey.Text = FieldContextData.OCRFieldName?.Truncate(60, true); // Form field name
                    else
                        txtOCROriginalKey.Text = FieldContextData.OCRFieldName;

                    txtOCROriginalValue.Text = FieldContextData.OCRFieldValue;

                    dgvEdit.AutoSize = true;
                    dgvEdit.Columns["Name"].Width = Convert.ToInt32(dgvEdit.Width * .38);
                    dgvEdit.Columns["Value"].Width = Convert.ToInt32(dgvEdit.Width * .6);
                    dgvEdit.Columns["Name"].ReadOnly = true;
                    dgvEdit.Columns["Value"].ReadOnly = false;
                    dgvEdit.Columns["Type"].Visible = false;
                    dgvEdit.Columns["MappedPropertyInfo"].Visible = false;
                }
            }
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

        void EmphasizeFields(Graphics graphics, TextractElements selectedElemets)
        {

            this.TextractClaimImage
                .Page?.Form.Fields
                .ForEach(f =>
                {
                    if (selectedElemets.HasFlag(TextractElements.Field))
                    {
                        var c = f.Match == null ? TextractClaimImageStyle.notMatchedFieldColor 
                            : ( f.MappedToClaim ? TextractClaimImageStyle.mappedFieldColor 
                                                : TextractClaimImageStyle.matchedFieldColor );
                        HighlightBoundingBox(graphics, f.Geometry.BoundingBox, c, defaultGauge, defaultOpacity, f.UpdateToClaim);
                        //HighlightBoundingBox(graphics, f?.Geometry?.BoundingBox, TextractClaimImageStyle.matchedFieldColor);
                    }

                    if (selectedElemets.HasFlag(TextractElements.FieldKey))
                    {
                        HighlightBoundingBox(graphics, f.Key?.Geometry.BoundingBox, TextractClaimImageStyle.matchedFieldKeyColor);
                    }
                    if (selectedElemets.HasFlag(TextractElements.FieldValue))
                    {
                        var c = f.Match == null ? TextractClaimImageStyle.notMatchedFieldColor 
                            : ( f.MappedToClaim ? TextractClaimImageStyle.mappedFieldValueColor 
                                                : TextractClaimImageStyle.matchedFieldValueColor );
                        HighlightBoundingBox(graphics, f.Value?.Geometry.BoundingBox, c, defaultGauge, defaultOpacity, f.UpdateToClaim);
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
                    .Rows.ForEach(r => r.Cells.ForEach(c =>
                    {
                        HighlightBoundingBox(graphics, c?.Geometry.BoundingBox, c.MappedToClaim ? TextractClaimImageStyle.mappedCellColor : TextractClaimImageStyle.matchedCellColor);
                    }));
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

        void HighlightBoundingCircle(Graphics graphics, PointF? center, Color color, float radius = defaultGauge, int opacity = defaultOpacity, bool fill = false)
        {
            if (center == null) return;

            PointF imagePosition = this.TextractClaimImage.ImagePosition(center.Value);
            PointF adjustedPosition = imageBox.GetOffsetPoint(imagePosition);
            SizeF scaledRadius = imageBox.GetScaledSize(new SizeF(radius, 0));

            if (fill)
            {
                var c = Color.FromArgb((int)Math.Round(255.0 * (opacity / 100.0)), color.R, color.G, color.B);

                using (Brush b = new SolidBrush(c))
                {
                    graphics.FillCircle(b, adjustedPosition.X, adjustedPosition.Y, scaledRadius.Width);
                }
            }
            else
                using (Pen pen = new Pen(color))
                {
                    graphics.DrawCircle(pen, adjustedPosition.X, adjustedPosition.Y, scaledRadius.Width);
                }

            //Logger.TraceVerbose("Ideal ({0}, {1})\tAdjusted ({2}, {3})", center.Value.X, center.Value.Y, adjustedPosition.X, adjustedPosition.Y);
        }

        void HighlightBoundingBox(Graphics graphics, Model.BoundingBox box, Color color, float gauge = defaultGauge, int opacity = defaultOpacity, bool fill = false)
        {
            if (box == null) return;

            Model.BoundingBox boundingBox = this.TextractClaimImage.ImagePosition(box);
            RectangleF adjustedPosition = imageBox.GetOffsetRectangle(boundingBox.Left, boundingBox.Top, boundingBox.Width, boundingBox.Height);
            SizeF scaledGauge = imageBox.GetScaledSize(new SizeF(gauge, 0));

            if (fill)
            {
                var c = Color.FromArgb((int)Math.Round(255.0 * (opacity / 100.0)), color.R, color.G, color.B);

                using (SolidBrush brush = new SolidBrush(c))
                {
                    graphics.FillRectangle(brush, adjustedPosition.Left, adjustedPosition.Top, adjustedPosition.Width, adjustedPosition.Height);
                }
            }
            else
                using (Pen pen = new Pen(color, scaledGauge.Width))
                {
                    graphics.DrawRectangle(pen, adjustedPosition.Left, adjustedPosition.Top, adjustedPosition.Width, adjustedPosition.Height);
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
            mainSplitContainer.Panel2Collapsed = false;

            MouseEventArgs me = (MouseEventArgs)e;

            PointF location = imageBox.PointToImage(me.Location);
            PointF identPos = TextractClaimImage.ImageIdentityPosition(this.TextractClaimImage.SourceImage, location);

            if (me.Button == MouseButtons.Right)
            {
                this.FieldContextData = ContextData.GetContext(this.TextractClaimImage, identPos, this.SelectedTextractElements);
                DisplayOCRContext();
            }

            if (me.Button == MouseButtons.Left)
            {
                this.FieldContextData = ContextData.GetContext(this.TextractClaimImage, identPos, this.SelectedTextractElements);
                if (editMode == EditMode.EditCoordinates)
                    DisplayCoordinatesContext();

                if (editMode == EditMode.EditField)
                    DisplayClaimFieldContext();
            }
        }
        
        private void imageBox_DoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            PointF location = imageBox.PointToImage(me.Location);

            if (me.Button == MouseButtons.Left)
            {
                PointF identPos = TextractClaimImage.ImageIdentityPosition(this.TextractClaimImage.SourceImage, new PointF { X = location.X, Y = location.Y });

                this.FieldContextData = ContextData.GetContext(this.TextractClaimImage, new PointF { X = identPos.X, Y = identPos.Y }, this.SelectedTextractElements);
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

                txtDrawingResults.Text
                    = txtDrawingResults.Text
                    + Environment.NewLine
                    + FormatRectangle(rc)
                    + Environment.NewLine
                    + FormatRectangleF(getIdentityRectangle(rc));

                imageBox.Invalidate();
            }
        }

        private void imageBox_Paint(object sender, PaintEventArgs e)
        {
            if (this.TextractClaimImage != null)
            {
                if (HighlightMatchedClaimsData)
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

            SizeF scaledGauge = imageBox.GetScaledSize(new SizeF(defaultGauge, 0));

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
                HighlightBoundingBox(e.Graphics, this.currentFieldBoundingBox, Color.Orange,3,25, true);

            if (namedCoordinatesRectangles.Count > 0)
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
            this.UpdateStatusBar();
        }

        private void imageBox_ZoomLevelsChanged(object sender, EventArgs e)
        {
            this.FillZoomLevelSelector();
        }

        #endregion

        #region Status Methods

        void UpdateCursorPosition(Point location)
        {
            if (TextractClaimImage == null)
                return;

            Point point = imageBox.PointToImage(location);
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

        #region Drawing Mode Methods

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
            PointF offset = TextractClaimImage.ImageIdentityPosition(this.TextractClaimImage.SourceImage, new Point(rectangle.Width, rectangle.Height));

            return new RectangleF(orign.X, orign.Y, offset.X, offset.Y);
        }

        Rectangle getRectangleFromBoundingBox(Model.BoundingBox boundingBox)
        {
            var adjusted = TextractClaimImage.ImagePosition(boundingBox);
            return new Rectangle((int)adjusted.Left, (int)adjusted.Top, (int)adjusted.Width, (int)adjusted.Height);
        }
        #endregion

        #region ToolStrip, Menus & Buttons

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
            ReloadImage(ComboBox_NamedCoordinates.Text);
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
                case "EditClaim":
                    editMode = EditMode.EditClaim;
                    break;
                case "EditCoordinates":
                    editMode = EditMode.EditCoordinates;
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

        private void tsButton_Drawing_Click(object sender, EventArgs e)
        {
            ToolStripButton drawingButton = (ToolStripButton)sender;

            drawingButton.Text = drawingButton.Checked ? "Drawing On" : "Drawing Off";

            if (drawingButton.Checked)
                tabControl.SelectTab("tabPage_Drawing");

            //if (!drawingButton.Checked && manualDrawnRectangles.Count() > 0)
            //{
            //    string desc = "";
            //    manualDrawnRectangles.ForEach(r =>
            //    {
            //        RectangleF f = getIdentityRectangle(r);
            //        desc += string.Format("({0},{1}) [{2} x {3}] => ({4},{5}) [{6} x {7}]" + Environment.NewLine
            //                                , r.Left, r.Top, r.Height, r.Width
            //                                , f.Left, f.Top, f.Height, f.Width
            //                                );
            //    });

            //    txtDrawingResults.Text = desc;
            //}
        }

        private void tsButton_MatchedFields_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripButton tsButton = (ToolStripButton)sender;

            this.HighlightMatchedClaimsData = tsButton.Checked;

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

        private void btnInfoPanel_Click(object sender, EventArgs e)
        {
            
            Button btn = (Button)sender;

            switch (btn.Tag)
            {
                case "Hide":
                    mainSplitContainer.Panel2Collapsed = true;
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
                default:
                    break;
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
                    Clipboard.SetText(txtDrawingResults.Text);
                    break;

                //case "Update":
                //    UpdateDrawingContextNamedCoordinate();
                //    break;

                default:
                    break;
            }
        }

        private void btnNavigation_Click(object sender, EventArgs e)
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

            var field = this.TextractClaimImage.Page.Form.Fields.FirstOrDefault(f => f.TabOrder == currentFieldTabOrder);

            if (field == null)
            {
                var coord = this.TextractClaimImage.Cache.TextractDocument.NamedCoordinates.SingleOrDefault(n => n.TabOrder == currentFieldTabOrder);
                field = new Field(coord);
            }
            this.FieldContextData = ContextData.GetContext(this.TextractClaimImage, field, this.SelectedTextractElements);

            //if (field?.Geometry != null && field.Geometry is NewGeometry)
            if (chkHighlightCurrentField.Checked && field?.Geometry != null)
            {
                PointF p = TextractClaimImage.ImagePosition(((NewGeometry)field.Geometry).Center);
                this.imageBox.CenterAt(p.X, p.Y);
            }

            DisplayClaimFieldContext(chkHighlightCurrentField.Checked);
            this.imageBox.Invalidate();
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
                    PopulateContextTableDataGridView();
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
