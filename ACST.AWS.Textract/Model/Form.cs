using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Amazon.Textract.Model;
using System.Drawing;

namespace ACST.AWS.Textract.Model
{
    public class Form {

        #region Properties & Fields
        
        public List<Field> Fields { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        private Dictionary<string, Field> fieldMap;

        //[JsonIgnore]
        //[XmlIgnore]
        public List<Field> MatchedFields { get { return this.Fields.Where(f => f.Key.HasMatch).ToList(); } }
        //public Dictionary<string, Field> MatchedFieldMap;
        #endregion

        public Form() {
			this.Fields = new List<Field>();
			this.fieldMap = new Dictionary<string, Field>();
        }

		public void AddField(Field field) {

			this.Fields.Add(field);
            //this.fieldMap.Add(field.Key.ToString(), field);

            if (this.fieldMap.ContainsKey(field.Key.ToString()))
                this.fieldMap.Add(field.Key.ToString() + "-" + field.Key.Id, field);
            else
                this.fieldMap.Add(field.Key.ToString(), field);

            //if(field.Key.HasMatch)
            //{
            //    this.MatchedFieldMap.Add(field.Key.Match, field);
            //}
        }

        public Field GetFieldByFieldCoordinate(PointF coordinate)
        {
            if (this == null) return null;

            return this.Fields
                .SingleOrDefault(f =>
                    f.Geometry.ContainsPointF(coordinate))
                ;
        }

        public Field GetFieldByFieldValueCoordinate(PointF coordinate)
        {
            if (this == null) return null;

            return this.Fields
                .SingleOrDefault(f => 
                    f.Value != null 
                    && f.Geometry.ContainsPointF(coordinate))
                ;
        }

        public Field GetFieldByKey(string key) {
			return this.fieldMap.GetValueOrDefault(key);
		}

        public Field GetFieldById(string id)
        {
            return this.Fields.SingleOrDefault(s => s.Id == id);
        }

        public List<Field> SearchFieldsByKey(string key) {
			return this.Fields.FindAll(f => f.Key.ToString().ToLower().Contains(key.ToLower()));
		}

		public override string ToString() {
			return string.Join("\n", this.Fields);
		}
	}
}