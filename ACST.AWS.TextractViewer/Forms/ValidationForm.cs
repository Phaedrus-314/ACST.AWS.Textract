
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
    //using Model = Amazon.Textract.Model;
    using ACST.AWS.TextractClaimMapper.OCR;

    public partial class ValidationForm : Form
    {

        #region Fields & Properties

        ContextData FieldContextData { get; set; }

        public IEnumerable<ValidationResult> Results { get; set; }

        internal TextractClaimImage TextractClaimImage { get; set; }
        #endregion

        public ValidationForm()
        {
            InitializeComponent();
        }

        public ValidationForm(TextractClaimImage textractClaimImage) //, IEnumerable<ValidationResult> results) 
            : this()
        {
            this.TextractClaimImage = textractClaimImage;
            this.Results = this.TextractClaimImage.ValidationResults;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Results.ToList().ForEach(r => textBox1.Text = textBox1.Text + $"{r.Result}{Environment.NewLine}");

            BindingSource source = new BindingSource();

            dgvValidation.Columns.Clear();
            dgvValidation.Rows.Clear();
            dgvValidation.DataSource = null;
            dgvValidation.Refresh();

            dgvValidation.AutoGenerateColumns = false;
            dgvValidation.AutoSize = true;
            dgvValidation.RowHeadersVisible = false;
            dgvValidation.DataSource = source;

            DataGridViewColumn(dgvValidation, "Group", new DataGridViewTextBoxColumn(), 1);
            DataGridViewColumn(dgvValidation, "Name", new DataGridViewTextBoxColumn(), 2);
            DataGridViewColumn(dgvValidation, "Value", new DataGridViewTextBoxColumn(), 3);
            DataGridViewColumn(dgvValidation, "Type", new DataGridViewTextBoxColumn(), 4);
            DataGridViewColumn(dgvValidation, "MappedPropertyInfo", new DataGridViewTextBoxColumn(), 5);

            foreach (var result in Results)
            {
                textBox1.Text = textBox1.Text + $"{result.Result}{Environment.NewLine}";

                var field = this.TextractClaimImage.Page.Form.Fields.FirstOrDefault(f => f.Match == result.Name);

                if (field == null)
                {
                    var coord = this.TextractClaimImage.Cache.TextractDocument.NamedCoordinates.SingleOrDefault(n => n.Name == result.Name);
                    field = new Textract.Model.Field(coord);
                }
                this.FieldContextData = ContextData.GetContext(this.TextractClaimImage, field, TextractElements.FieldValue | TextractElements.FieldKey);

                DisplayClaimFieldContext(source, result.Group);
            }

            if (source.Count > 0)
            {
                //tabControl.SelectTab("tabPage_EditField");

                //this.currentFieldTabOrder = FieldContextData.Element.TabOrder;
                //txtCurrentFieldNo.Text = this.currentFieldTabOrder.ToString();

                //if (highlightField)
                //    this.currentFieldBoundingBox = FieldContextData.Element.Geometry?.BoundingBox;

                //if (mainSplitContainer.Orientation == Orientation.Vertical)
                //    txtOCROriginalKey.Text = FieldContextData.OCRFieldName?.Truncate(60, true); // Form field name
                //else
                //    txtOCROriginalKey.Text = FieldContextData.OCRFieldName;

                //txtOCROriginalValue.Text = FieldContextData.OCRFieldValue;

                dgvValidation.AutoSize = true;
                dgvValidation.Columns["Name"].Width = Convert.ToInt32(dgvValidation.Width * .38);
                dgvValidation.Columns["Value"].Width = Convert.ToInt32(dgvValidation.Width * .6);
                dgvValidation.Columns["Group"].ReadOnly = true;
                dgvValidation.Columns["Name"].ReadOnly = true;
                dgvValidation.Columns["Value"].ReadOnly = false;
                dgvValidation.Columns["Type"].Visible = false;
                dgvValidation.Columns["MappedPropertyInfo"].Visible = false;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

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

        void DisplayClaimFieldContext(BindingSource source, string group, bool highlightField = false)
        {
            //if (this.FieldContextData.ElementType == "Table" & this.FieldContextData.CellElements?.Count() > 0)
            //{
            //    tabControl.SelectTab("tabPage_EditTable");
            //}
            //else
            if (this.FieldContextData.Element != null)
            {
                //this.namedCoordinatesRectangles.Clear();
                //this.currentFieldBoundingBox = null;

                //BindingSource source = new BindingSource();

                //dgvEdit.Columns.Clear();
                //dgvEdit.Rows.Clear();
                //dgvEdit.DataSource = null;
                //dgvEdit.Refresh();

                //dgvEdit.AutoGenerateColumns = false;
                //dgvEdit.AutoSize = true;
                //dgvEdit.RowHeadersVisible = false;
                //dgvEdit.DataSource = source;

                switch (this.FieldContextData.PropertyType)
                {
                    case "String":
                        //DataGridViewColumn(dgvEdit, "Name", new DataGridViewTextBoxColumn(), 1);
                        //DataGridViewColumn(dgvEdit, "Value", new DataGridViewTextBoxColumn(), 2);
                        //DataGridViewColumn(dgvEdit, "Type", new DataGridViewTextBoxColumn(), 3);
                        //DataGridViewColumn(dgvEdit, "MappedPropertyInfo", new DataGridViewTextBoxColumn(), 4);

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
                            //DataGridViewColumn(dgvEdit, "Name", new DataGridViewTextBoxColumn(), 1);
                            //DataGridViewColumn(dgvEdit, "Value", new DataGridViewTextBoxColumn(), 2);
                            //DataGridViewColumn(dgvEdit, "Type", new DataGridViewTextBoxColumn(), 3);
                            //DataGridViewColumn(dgvEdit, "MappedPropertyInfo", new DataGridViewTextBoxColumn(), 4);

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

                    //case "Table":
                    //    tabControl.SelectTab("tabPage_EditTable");
                    //    break;

                    default:
                        Logger.TraceWarning("Claim propery type: '{0}', does not yet support update.", this.FieldContextData.PropertyType);
                        //dgvEdit.DataSource = null;
                        ////txtOCROriginalKey.Text = "";
                        ////txtOCROriginalValue.Text = "";
                        ////UpdateStatusBar("OCR Field not mapped...", 3);
                        break;
                }

                //if (source.Count > 0)
                //{
                //    //tabControl.SelectTab("tabPage_EditField");

                //    //this.currentFieldTabOrder = FieldContextData.Element.TabOrder;
                //    //txtCurrentFieldNo.Text = this.currentFieldTabOrder.ToString();

                //    //if (highlightField)
                //    //    this.currentFieldBoundingBox = FieldContextData.Element.Geometry?.BoundingBox;

                //    //if (mainSplitContainer.Orientation == Orientation.Vertical)
                //    //    txtOCROriginalKey.Text = FieldContextData.OCRFieldName?.Truncate(60, true); // Form field name
                //    //else
                //    //    txtOCROriginalKey.Text = FieldContextData.OCRFieldName;

                //    //txtOCROriginalValue.Text = FieldContextData.OCRFieldValue;

                //    dgvEdit.AutoSize = true;
                //    dgvEdit.Columns["Name"].Width = Convert.ToInt32(dgvEdit.Width * .38);
                //    dgvEdit.Columns["Value"].Width = Convert.ToInt32(dgvEdit.Width * .6);
                //    dgvEdit.Columns["Name"].ReadOnly = true;
                //    dgvEdit.Columns["Value"].ReadOnly = false;
                //    dgvEdit.Columns["Type"].Visible = false;
                //    dgvEdit.Columns["MappedPropertyInfo"].Visible = false;
                //}
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
