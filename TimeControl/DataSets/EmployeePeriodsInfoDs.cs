﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.573
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

namespace Kesco.App.Web.TimeControl.DataSets
{
    using System;
    using System.Data;
    using System.Xml;
    using System.Runtime.Serialization;
    
    
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.ComponentModel.ToolboxItem(true)]
    public class EmployeePeriodsInfoDs : DataSet {
        
        private СотрудникиDataTable tableСотрудники;
        
        private ПроходыСотрудниковDataTable tableПроходыСотрудников;
        
        private ДоступВИнтернетDataTable tableДоступВИнтернет;
        
        public EmployeePeriodsInfoDs() {
            this.InitClass();
            System.ComponentModel.CollectionChangeEventHandler schemaChangedHandler = new System.ComponentModel.CollectionChangeEventHandler(this.SchemaChanged);
            this.Tables.CollectionChanged += schemaChangedHandler;
            this.Relations.CollectionChanged += schemaChangedHandler;
        }
        
        protected EmployeePeriodsInfoDs(SerializationInfo info, StreamingContext context) {
            string strSchema = ((string)(info.GetValue("XmlSchema", typeof(string))));
            if ((strSchema != null)) {
                DataSet ds = new DataSet();
                ds.ReadXmlSchema(new XmlTextReader(new System.IO.StringReader(strSchema)));
                if ((ds.Tables["Сотрудники"] != null)) {
                    this.Tables.Add(new СотрудникиDataTable(ds.Tables["Сотрудники"]));
                }
                if ((ds.Tables["ПроходыСотрудников"] != null)) {
                    this.Tables.Add(new ПроходыСотрудниковDataTable(ds.Tables["ПроходыСотрудников"]));
                }
                if ((ds.Tables["ДоступВИнтернет"] != null)) {
                    this.Tables.Add(new ДоступВИнтернетDataTable(ds.Tables["ДоступВИнтернет"]));
                }
                this.DataSetName = ds.DataSetName;
                this.Prefix = ds.Prefix;
                this.Namespace = ds.Namespace;
                this.Locale = ds.Locale;
                this.CaseSensitive = ds.CaseSensitive;
                this.EnforceConstraints = ds.EnforceConstraints;
                this.Merge(ds, false, System.Data.MissingSchemaAction.Add);
                this.InitVars();
            }
            else {
                this.InitClass();
            }
            this.GetSerializationData(info, context);
            System.ComponentModel.CollectionChangeEventHandler schemaChangedHandler = new System.ComponentModel.CollectionChangeEventHandler(this.SchemaChanged);
            this.Tables.CollectionChanged += schemaChangedHandler;
            this.Relations.CollectionChanged += schemaChangedHandler;
        }
        
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Content)]
        public СотрудникиDataTable Сотрудники {
            get {
                return this.tableСотрудники;
            }
        }
        
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Content)]
        public ПроходыСотрудниковDataTable ПроходыСотрудников {
            get {
                return this.tableПроходыСотрудников;
            }
        }
        
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Content)]
        public ДоступВИнтернетDataTable ДоступВИнтернет {
            get {
                return this.tableДоступВИнтернет;
            }
        }
        
        public override DataSet Clone() {
            EmployeePeriodsInfoDs cln = ((EmployeePeriodsInfoDs)(base.Clone()));
            cln.InitVars();
            return cln;
        }
        
        protected override bool ShouldSerializeTables() {
            return false;
        }
        
        protected override bool ShouldSerializeRelations() {
            return false;
        }
        
        protected override void ReadXmlSerializable(XmlReader reader) {
            this.Reset();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);
            if ((ds.Tables["Сотрудники"] != null)) {
                this.Tables.Add(new СотрудникиDataTable(ds.Tables["Сотрудники"]));
            }
            if ((ds.Tables["ПроходыСотрудников"] != null)) {
                this.Tables.Add(new ПроходыСотрудниковDataTable(ds.Tables["ПроходыСотрудников"]));
            }
            if ((ds.Tables["ДоступВИнтернет"] != null)) {
                this.Tables.Add(new ДоступВИнтернетDataTable(ds.Tables["ДоступВИнтернет"]));
            }
            this.DataSetName = ds.DataSetName;
            this.Prefix = ds.Prefix;
            this.Namespace = ds.Namespace;
            this.Locale = ds.Locale;
            this.CaseSensitive = ds.CaseSensitive;
            this.EnforceConstraints = ds.EnforceConstraints;
            this.Merge(ds, false, System.Data.MissingSchemaAction.Add);
            this.InitVars();
        }
        
        protected override System.Xml.Schema.XmlSchema GetSchemaSerializable() {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            this.WriteXmlSchema(new XmlTextWriter(stream, null));
            stream.Position = 0;
            return System.Xml.Schema.XmlSchema.Read(new XmlTextReader(stream), null);
        }
        
        internal void InitVars() {
            this.tableСотрудники = ((СотрудникиDataTable)(this.Tables["Сотрудники"]));
            if ((this.tableСотрудники != null)) {
                this.tableСотрудники.InitVars();
            }
            this.tableПроходыСотрудников = ((ПроходыСотрудниковDataTable)(this.Tables["ПроходыСотрудников"]));
            if ((this.tableПроходыСотрудников != null)) {
                this.tableПроходыСотрудников.InitVars();
            }
            this.tableДоступВИнтернет = ((ДоступВИнтернетDataTable)(this.Tables["ДоступВИнтернет"]));
            if ((this.tableДоступВИнтернет != null)) {
                this.tableДоступВИнтернет.InitVars();
            }
        }
        
        private void InitClass() {
            this.DataSetName = "EmployeePeriodsInfoDs";
            this.Prefix = "";
            this.Namespace = "http://tempuri.org/EmployeePeriodsInfoDs.xsd";
            this.Locale = new System.Globalization.CultureInfo("en-US");
            this.CaseSensitive = false;
            this.EnforceConstraints = true;
            this.tableСотрудники = new СотрудникиDataTable();
            this.Tables.Add(this.tableСотрудники);
            this.tableПроходыСотрудников = new ПроходыСотрудниковDataTable();
            this.Tables.Add(this.tableПроходыСотрудников);
            this.tableДоступВИнтернет = new ДоступВИнтернетDataTable();
            this.Tables.Add(this.tableДоступВИнтернет);
        }
        
        private bool ShouldSerializeСотрудники() {
            return false;
        }
        
        private bool ShouldSerializeПроходыСотрудников() {
            return false;
        }
        
        private bool ShouldSerializeДоступВИнтернет() {
            return false;
        }
        
        private void SchemaChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e) {
            if ((e.Action == System.ComponentModel.CollectionChangeAction.Remove)) {
                this.InitVars();
            }
        }
        
        public delegate void СотрудникиRowChangeEventHandler(object sender, СотрудникиRowChangeEvent e);
        
        public delegate void ПроходыСотрудниковRowChangeEventHandler(object sender, ПроходыСотрудниковRowChangeEvent e);
        
        public delegate void ДоступВИнтернетRowChangeEventHandler(object sender, ДоступВИнтернетRowChangeEvent e);
        
        [System.Diagnostics.DebuggerStepThrough()]
        public class СотрудникиDataTable : DataTable, System.Collections.IEnumerable {
            
            private DataColumn columnКодСотрудника;
            
            private DataColumn columnСотрудник;
            
            private DataColumn columnEmployee;

            private DataColumn columnКодЛица;
            
            internal СотрудникиDataTable() : 
                    base("Сотрудники") {
                this.InitClass();
            }
            
            internal СотрудникиDataTable(DataTable table) : 
                    base(table.TableName) {
                if ((table.CaseSensitive != table.DataSet.CaseSensitive)) {
                    this.CaseSensitive = table.CaseSensitive;
                }
                if ((table.Locale.ToString() != table.DataSet.Locale.ToString())) {
                    this.Locale = table.Locale;
                }
                if ((table.Namespace != table.DataSet.Namespace)) {
                    this.Namespace = table.Namespace;
                }
                this.Prefix = table.Prefix;
                this.MinimumCapacity = table.MinimumCapacity;
                this.DisplayExpression = table.DisplayExpression;
            }
            
            [System.ComponentModel.Browsable(false)]
            public int Count {
                get {
                    return this.Rows.Count;
                }
            }
            
            internal DataColumn КодСотрудникаColumn {
                get {
                    return this.columnКодСотрудника;
                }
            }
            
            internal DataColumn СотрудникColumn {
                get {
                    return this.columnСотрудник;
                }
            }
            
            internal DataColumn EmployeeColumn {
                get {
                    return this.columnEmployee;
                }
            }

            internal DataColumn КодЛицаColumn
            {
                get
                {
                    return this.columnКодЛица;
                }
            }
            
            public СотрудникиRow this[int index] {
                get {
                    return ((СотрудникиRow)(this.Rows[index]));
                }
            }
            
            public event СотрудникиRowChangeEventHandler СотрудникиRowChanged;
            
            public event СотрудникиRowChangeEventHandler СотрудникиRowChanging;
            
            public event СотрудникиRowChangeEventHandler СотрудникиRowDeleted;
            
            public event СотрудникиRowChangeEventHandler СотрудникиRowDeleting;
            
            public void AddСотрудникиRow(СотрудникиRow row) {
                this.Rows.Add(row);
            }

            public СотрудникиRow AddСотрудникиRow(int КодСотрудника, string Сотрудник, string Employee, string КодЛица)
            {
                СотрудникиRow rowСотрудникиRow = ((СотрудникиRow)(this.NewRow()));
                rowСотрудникиRow.ItemArray = new object[] {
                        КодСотрудника,
                        Сотрудник,
                        Employee,
                        КодЛица};
                this.Rows.Add(rowСотрудникиRow);
                return rowСотрудникиRow;
            }
            
            public System.Collections.IEnumerator GetEnumerator() {
                return this.Rows.GetEnumerator();
            }
            
            public override DataTable Clone() {
                СотрудникиDataTable cln = ((СотрудникиDataTable)(base.Clone()));
                cln.InitVars();
                return cln;
            }
            
            protected override DataTable CreateInstance() {
                return new СотрудникиDataTable();
            }
            
            internal void InitVars() {
                this.columnКодСотрудника = this.Columns["КодСотрудника"];
                this.columnСотрудник = this.Columns["Сотрудник"];
                this.columnEmployee = this.Columns["Employee"];
                this.columnКодЛица = this.Columns["КодЛица"];
            }
            
            private void InitClass() {
                this.columnКодСотрудника = new DataColumn("КодСотрудника", typeof(int), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnКодСотрудника);
                this.columnСотрудник = new DataColumn("Сотрудник", typeof(string), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnСотрудник);
                this.columnEmployee = new DataColumn("Employee", typeof(string), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnEmployee);
                this.columnКодЛица = new DataColumn("КодЛица", typeof(string), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnКодЛица);
                this.Constraints.Add(new UniqueConstraint("EmployeePeriodsInfoDsKey1", new DataColumn[] {
                                this.columnКодСотрудника}, false));
                this.columnКодСотрудника.AllowDBNull = false;
                this.columnКодСотрудника.Unique = true;
                this.columnСотрудник.AllowDBNull = false;
            }
            
            public СотрудникиRow NewСотрудникиRow() {
                return ((СотрудникиRow)(this.NewRow()));
            }
            
            protected override DataRow NewRowFromBuilder(DataRowBuilder builder) {
                return new СотрудникиRow(builder);
            }
            
            protected override System.Type GetRowType() {
                return typeof(СотрудникиRow);
            }
            
            protected override void OnRowChanged(DataRowChangeEventArgs e) {
                base.OnRowChanged(e);
                if ((this.СотрудникиRowChanged != null)) {
                    this.СотрудникиRowChanged(this, new СотрудникиRowChangeEvent(((СотрудникиRow)(e.Row)), e.Action));
                }
            }
            
            protected override void OnRowChanging(DataRowChangeEventArgs e) {
                base.OnRowChanging(e);
                if ((this.СотрудникиRowChanging != null)) {
                    this.СотрудникиRowChanging(this, new СотрудникиRowChangeEvent(((СотрудникиRow)(e.Row)), e.Action));
                }
            }
            
            protected override void OnRowDeleted(DataRowChangeEventArgs e) {
                base.OnRowDeleted(e);
                if ((this.СотрудникиRowDeleted != null)) {
                    this.СотрудникиRowDeleted(this, new СотрудникиRowChangeEvent(((СотрудникиRow)(e.Row)), e.Action));
                }
            }
            
            protected override void OnRowDeleting(DataRowChangeEventArgs e) {
                base.OnRowDeleting(e);
                if ((this.СотрудникиRowDeleting != null)) {
                    this.СотрудникиRowDeleting(this, new СотрудникиRowChangeEvent(((СотрудникиRow)(e.Row)), e.Action));
                }
            }
            
            public void RemoveСотрудникиRow(СотрудникиRow row) {
                this.Rows.Remove(row);
            }
        }
        
        [System.Diagnostics.DebuggerStepThrough()]
        public class СотрудникиRow : DataRow {
            
            private СотрудникиDataTable tableСотрудники;
            
            internal СотрудникиRow(DataRowBuilder rb) : 
                    base(rb) {
                this.tableСотрудники = ((СотрудникиDataTable)(this.Table));
            }
            
            public int КодСотрудника {
                get {
                    return ((int)(this[this.tableСотрудники.КодСотрудникаColumn]));
                }
                set {
                    this[this.tableСотрудники.КодСотрудникаColumn] = value;
                }
            }
            
            public string Сотрудник {
                get {
                    return ((string)(this[this.tableСотрудники.СотрудникColumn]));
                }
                set {
                    this[this.tableСотрудники.СотрудникColumn] = value;
                }
            }
            
            public string Employee {
                get {
                    try {
                        return ((string)(this[this.tableСотрудники.EmployeeColumn]));
                    }
                    catch (InvalidCastException e) {
                        throw new StrongTypingException("Cannot get value because it is DBNull.", e);
                    }
                }
                set {
                    this[this.tableСотрудники.EmployeeColumn] = value;
                }
            }

            public string КодЛица
            {
                get
                {
                    return ((string)(this[this.tableСотрудники.КодЛицаColumn]));
                }
                set
                {
                    this[this.tableСотрудники.КодЛицаColumn] = value;
                }
            }
            
            public bool IsEmployeeNull() {
                return this.IsNull(this.tableСотрудники.EmployeeColumn);
            }
            
            public void SetEmployeeNull() {
                this[this.tableСотрудники.EmployeeColumn] = System.Convert.DBNull;
            }
        }
        
        [System.Diagnostics.DebuggerStepThrough()]
        public class СотрудникиRowChangeEvent : EventArgs {
            
            private СотрудникиRow eventRow;
            
            private DataRowAction eventAction;
            
            public СотрудникиRowChangeEvent(СотрудникиRow row, DataRowAction action) {
                this.eventRow = row;
                this.eventAction = action;
            }
            
            public СотрудникиRow Row {
                get {
                    return this.eventRow;
                }
            }
            
            public DataRowAction Action {
                get {
                    return this.eventAction;
                }
            }
        }
        
        [System.Diagnostics.DebuggerStepThrough()]
        public class ПроходыСотрудниковDataTable : DataTable, System.Collections.IEnumerable {
            
            private DataColumn columnКодПроходаСотрудника;
            
            private DataColumn columnКодСотрудника;
            
            private DataColumn columnКогда;
            
            private DataColumn columnКодРасположения;
            
            internal ПроходыСотрудниковDataTable() : 
                    base("ПроходыСотрудников") {
                this.InitClass();
            }
            
            internal ПроходыСотрудниковDataTable(DataTable table) : 
                    base(table.TableName) {
                if ((table.CaseSensitive != table.DataSet.CaseSensitive)) {
                    this.CaseSensitive = table.CaseSensitive;
                }
                if ((table.Locale.ToString() != table.DataSet.Locale.ToString())) {
                    this.Locale = table.Locale;
                }
                if ((table.Namespace != table.DataSet.Namespace)) {
                    this.Namespace = table.Namespace;
                }
                this.Prefix = table.Prefix;
                this.MinimumCapacity = table.MinimumCapacity;
                this.DisplayExpression = table.DisplayExpression;
            }
            
            [System.ComponentModel.Browsable(false)]
            public int Count {
                get {
                    return this.Rows.Count;
                }
            }
            
            internal DataColumn КодПроходаСотрудникаColumn {
                get {
                    return this.columnКодПроходаСотрудника;
                }
            }
            
            internal DataColumn КодСотрудникаColumn {
                get {
                    return this.columnКодСотрудника;
                }
            }
            
            internal DataColumn КогдаColumn {
                get {
                    return this.columnКогда;
                }
            }
            
            internal DataColumn КодРасположенияColumn {
                get {
                    return this.columnКодРасположения;
                }
            }
            
            public ПроходыСотрудниковRow this[int index] {
                get {
                    return ((ПроходыСотрудниковRow)(this.Rows[index]));
                }
            }
            
            public event ПроходыСотрудниковRowChangeEventHandler ПроходыСотрудниковRowChanged;
            
            public event ПроходыСотрудниковRowChangeEventHandler ПроходыСотрудниковRowChanging;
            
            public event ПроходыСотрудниковRowChangeEventHandler ПроходыСотрудниковRowDeleted;
            
            public event ПроходыСотрудниковRowChangeEventHandler ПроходыСотрудниковRowDeleting;
            
            public void AddПроходыСотрудниковRow(ПроходыСотрудниковRow row) {
                this.Rows.Add(row);
            }
            
            public ПроходыСотрудниковRow AddПроходыСотрудниковRow(int КодСотрудника, System.DateTime Когда, int КодРасположения) {
                ПроходыСотрудниковRow rowПроходыСотрудниковRow = ((ПроходыСотрудниковRow)(this.NewRow()));
                rowПроходыСотрудниковRow.ItemArray = new object[] {
                        null,
                        КодСотрудника,
                        Когда,
                        КодРасположения};
                this.Rows.Add(rowПроходыСотрудниковRow);
                return rowПроходыСотрудниковRow;
            }
            
            public System.Collections.IEnumerator GetEnumerator() {
                return this.Rows.GetEnumerator();
            }
            
            public override DataTable Clone() {
                ПроходыСотрудниковDataTable cln = ((ПроходыСотрудниковDataTable)(base.Clone()));
                cln.InitVars();
                return cln;
            }
            
            protected override DataTable CreateInstance() {
                return new ПроходыСотрудниковDataTable();
            }
            
            internal void InitVars() {
                this.columnКодПроходаСотрудника = this.Columns["КодПроходаСотрудника"];
                this.columnКодСотрудника = this.Columns["КодСотрудника"];
                this.columnКогда = this.Columns["Когда"];
                this.columnКодРасположения = this.Columns["КодРасположения"];
            }
            
            private void InitClass() {
                this.columnКодПроходаСотрудника = new DataColumn("КодПроходаСотрудника", typeof(int), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnКодПроходаСотрудника);
                this.columnКодСотрудника = new DataColumn("КодСотрудника", typeof(int), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnКодСотрудника);
                this.columnКогда = new DataColumn("Когда", typeof(System.DateTime), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnКогда);
                this.columnКодРасположения = new DataColumn("КодРасположения", typeof(int), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnКодРасположения);
                this.Constraints.Add(new UniqueConstraint("EmployeePeriodsInfoDsKey2", new DataColumn[] {
                                this.columnКодПроходаСотрудника}, false));
                this.columnКодПроходаСотрудника.AutoIncrement = true;
                this.columnКодПроходаСотрудника.AllowDBNull = false;
                this.columnКодПроходаСотрудника.ReadOnly = true;
                this.columnКодПроходаСотрудника.Unique = true;
                this.columnКодСотрудника.AllowDBNull = false;
                this.columnКогда.AllowDBNull = false;
            }
            
            public ПроходыСотрудниковRow NewПроходыСотрудниковRow() {
                return ((ПроходыСотрудниковRow)(this.NewRow()));
            }
            
            protected override DataRow NewRowFromBuilder(DataRowBuilder builder) {
                return new ПроходыСотрудниковRow(builder);
            }
            
            protected override System.Type GetRowType() {
                return typeof(ПроходыСотрудниковRow);
            }
            
            protected override void OnRowChanged(DataRowChangeEventArgs e) {
                base.OnRowChanged(e);
                if ((this.ПроходыСотрудниковRowChanged != null)) {
                    this.ПроходыСотрудниковRowChanged(this, new ПроходыСотрудниковRowChangeEvent(((ПроходыСотрудниковRow)(e.Row)), e.Action));
                }
            }
            
            protected override void OnRowChanging(DataRowChangeEventArgs e) {
                base.OnRowChanging(e);
                if ((this.ПроходыСотрудниковRowChanging != null)) {
                    this.ПроходыСотрудниковRowChanging(this, new ПроходыСотрудниковRowChangeEvent(((ПроходыСотрудниковRow)(e.Row)), e.Action));
                }
            }
            
            protected override void OnRowDeleted(DataRowChangeEventArgs e) {
                base.OnRowDeleted(e);
                if ((this.ПроходыСотрудниковRowDeleted != null)) {
                    this.ПроходыСотрудниковRowDeleted(this, new ПроходыСотрудниковRowChangeEvent(((ПроходыСотрудниковRow)(e.Row)), e.Action));
                }
            }
            
            protected override void OnRowDeleting(DataRowChangeEventArgs e) {
                base.OnRowDeleting(e);
                if ((this.ПроходыСотрудниковRowDeleting != null)) {
                    this.ПроходыСотрудниковRowDeleting(this, new ПроходыСотрудниковRowChangeEvent(((ПроходыСотрудниковRow)(e.Row)), e.Action));
                }
            }
            
            public void RemoveПроходыСотрудниковRow(ПроходыСотрудниковRow row) {
                this.Rows.Remove(row);
            }
        }
        
        [System.Diagnostics.DebuggerStepThrough()]
        public class ПроходыСотрудниковRow : DataRow {
            
            private ПроходыСотрудниковDataTable tableПроходыСотрудников;
            
            internal ПроходыСотрудниковRow(DataRowBuilder rb) : 
                    base(rb) {
                this.tableПроходыСотрудников = ((ПроходыСотрудниковDataTable)(this.Table));
            }
            
            public int КодПроходаСотрудника {
                get {
                    return ((int)(this[this.tableПроходыСотрудников.КодПроходаСотрудникаColumn]));
                }
                set {
                    this[this.tableПроходыСотрудников.КодПроходаСотрудникаColumn] = value;
                }
            }
            
            public int КодСотрудника {
                get {
                    return ((int)(this[this.tableПроходыСотрудников.КодСотрудникаColumn]));
                }
                set {
                    this[this.tableПроходыСотрудников.КодСотрудникаColumn] = value;
                }
            }
            
            public System.DateTime Когда {
                get {
                    return ((System.DateTime)(this[this.tableПроходыСотрудников.КогдаColumn]));
                }
                set {
                    this[this.tableПроходыСотрудников.КогдаColumn] = value;
                }
            }
            
            public int КодРасположения {
                get {
                    try {
                        return ((int)(this[this.tableПроходыСотрудников.КодРасположенияColumn]));
                    }
                    catch (InvalidCastException e) {
                        throw new StrongTypingException("Cannot get value because it is DBNull.", e);
                    }
                }
                set {
                    this[this.tableПроходыСотрудников.КодРасположенияColumn] = value;
                }
            }
            
            public bool IsКодРасположенияNull() {
                return this.IsNull(this.tableПроходыСотрудников.КодРасположенияColumn);
            }
            
            public void SetКодРасположенияNull() {
                this[this.tableПроходыСотрудников.КодРасположенияColumn] = System.Convert.DBNull;
            }
        }
        
        [System.Diagnostics.DebuggerStepThrough()]
        public class ПроходыСотрудниковRowChangeEvent : EventArgs {
            
            private ПроходыСотрудниковRow eventRow;
            
            private DataRowAction eventAction;
            
            public ПроходыСотрудниковRowChangeEvent(ПроходыСотрудниковRow row, DataRowAction action) {
                this.eventRow = row;
                this.eventAction = action;
            }
            
            public ПроходыСотрудниковRow Row {
                get {
                    return this.eventRow;
                }
            }
            
            public DataRowAction Action {
                get {
                    return this.eventAction;
                }
            }
        }
        
        [System.Diagnostics.DebuggerStepThrough()]
        public class ДоступВИнтернетDataTable : DataTable, System.Collections.IEnumerable {
            
            private DataColumn columnКодСотрудника;
            
            private DataColumn columnОт;
            
            private DataColumn columnПо;
            
            internal ДоступВИнтернетDataTable() : 
                    base("ДоступВИнтернет") {
                this.InitClass();
            }
            
            internal ДоступВИнтернетDataTable(DataTable table) : 
                    base(table.TableName) {
                if ((table.CaseSensitive != table.DataSet.CaseSensitive)) {
                    this.CaseSensitive = table.CaseSensitive;
                }
                if ((table.Locale.ToString() != table.DataSet.Locale.ToString())) {
                    this.Locale = table.Locale;
                }
                if ((table.Namespace != table.DataSet.Namespace)) {
                    this.Namespace = table.Namespace;
                }
                this.Prefix = table.Prefix;
                this.MinimumCapacity = table.MinimumCapacity;
                this.DisplayExpression = table.DisplayExpression;
            }
            
            [System.ComponentModel.Browsable(false)]
            public int Count {
                get {
                    return this.Rows.Count;
                }
            }
            
            internal DataColumn КодСотрудникаColumn {
                get {
                    return this.columnКодСотрудника;
                }
            }
            
            internal DataColumn ОтColumn {
                get {
                    return this.columnОт;
                }
            }
            
            internal DataColumn ПоColumn {
                get {
                    return this.columnПо;
                }
            }
            
            public ДоступВИнтернетRow this[int index] {
                get {
                    return ((ДоступВИнтернетRow)(this.Rows[index]));
                }
            }
            
            public event ДоступВИнтернетRowChangeEventHandler ДоступВИнтернетRowChanged;
            
            public event ДоступВИнтернетRowChangeEventHandler ДоступВИнтернетRowChanging;
            
            public event ДоступВИнтернетRowChangeEventHandler ДоступВИнтернетRowDeleted;
            
            public event ДоступВИнтернетRowChangeEventHandler ДоступВИнтернетRowDeleting;
            
            public void AddДоступВИнтернетRow(ДоступВИнтернетRow row) {
                this.Rows.Add(row);
            }
            
            public ДоступВИнтернетRow AddДоступВИнтернетRow(int КодСотрудника, System.DateTime От, System.DateTime По) {
                ДоступВИнтернетRow rowДоступВИнтернетRow = ((ДоступВИнтернетRow)(this.NewRow()));
                rowДоступВИнтернетRow.ItemArray = new object[] {
                        КодСотрудника,
                        От,
                        По};
                this.Rows.Add(rowДоступВИнтернетRow);
                return rowДоступВИнтернетRow;
            }
            
            public System.Collections.IEnumerator GetEnumerator() {
                return this.Rows.GetEnumerator();
            }
            
            public override DataTable Clone() {
                ДоступВИнтернетDataTable cln = ((ДоступВИнтернетDataTable)(base.Clone()));
                cln.InitVars();
                return cln;
            }
            
            protected override DataTable CreateInstance() {
                return new ДоступВИнтернетDataTable();
            }
            
            internal void InitVars() {
                this.columnКодСотрудника = this.Columns["КодСотрудника"];
                this.columnОт = this.Columns["От"];
                this.columnПо = this.Columns["По"];
            }
            
            private void InitClass() {
                this.columnКодСотрудника = new DataColumn("КодСотрудника", typeof(int), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnКодСотрудника);
                this.columnОт = new DataColumn("От", typeof(System.DateTime), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnОт);
                this.columnПо = new DataColumn("По", typeof(System.DateTime), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnПо);
                this.columnКодСотрудника.AllowDBNull = false;
                this.columnОт.AllowDBNull = false;
            }
            
            public ДоступВИнтернетRow NewДоступВИнтернетRow() {
                return ((ДоступВИнтернетRow)(this.NewRow()));
            }
            
            protected override DataRow NewRowFromBuilder(DataRowBuilder builder) {
                return new ДоступВИнтернетRow(builder);
            }
            
            protected override System.Type GetRowType() {
                return typeof(ДоступВИнтернетRow);
            }
            
            protected override void OnRowChanged(DataRowChangeEventArgs e) {
                base.OnRowChanged(e);
                if ((this.ДоступВИнтернетRowChanged != null)) {
                    this.ДоступВИнтернетRowChanged(this, new ДоступВИнтернетRowChangeEvent(((ДоступВИнтернетRow)(e.Row)), e.Action));
                }
            }
            
            protected override void OnRowChanging(DataRowChangeEventArgs e) {
                base.OnRowChanging(e);
                if ((this.ДоступВИнтернетRowChanging != null)) {
                    this.ДоступВИнтернетRowChanging(this, new ДоступВИнтернетRowChangeEvent(((ДоступВИнтернетRow)(e.Row)), e.Action));
                }
            }
            
            protected override void OnRowDeleted(DataRowChangeEventArgs e) {
                base.OnRowDeleted(e);
                if ((this.ДоступВИнтернетRowDeleted != null)) {
                    this.ДоступВИнтернетRowDeleted(this, new ДоступВИнтернетRowChangeEvent(((ДоступВИнтернетRow)(e.Row)), e.Action));
                }
            }
            
            protected override void OnRowDeleting(DataRowChangeEventArgs e) {
                base.OnRowDeleting(e);
                if ((this.ДоступВИнтернетRowDeleting != null)) {
                    this.ДоступВИнтернетRowDeleting(this, new ДоступВИнтернетRowChangeEvent(((ДоступВИнтернетRow)(e.Row)), e.Action));
                }
            }
            
            public void RemoveДоступВИнтернетRow(ДоступВИнтернетRow row) {
                this.Rows.Remove(row);
            }
        }
        
        [System.Diagnostics.DebuggerStepThrough()]
        public class ДоступВИнтернетRow : DataRow {
            
            private ДоступВИнтернетDataTable tableДоступВИнтернет;
            
            internal ДоступВИнтернетRow(DataRowBuilder rb) : 
                    base(rb) {
                this.tableДоступВИнтернет = ((ДоступВИнтернетDataTable)(this.Table));
            }
            
            public int КодСотрудника {
                get {
                    return ((int)(this[this.tableДоступВИнтернет.КодСотрудникаColumn]));
                }
                set {
                    this[this.tableДоступВИнтернет.КодСотрудникаColumn] = value;
                }
            }
            
            public System.DateTime От {
                get {
                    return ((System.DateTime)(this[this.tableДоступВИнтернет.ОтColumn]));
                }
                set {
                    this[this.tableДоступВИнтернет.ОтColumn] = value;
                }
            }
            
            public System.DateTime По {
                get {
                    try {
                        return ((System.DateTime)(this[this.tableДоступВИнтернет.ПоColumn]));
                    }
                    catch (InvalidCastException e) {
                        throw new StrongTypingException("Cannot get value because it is DBNull.", e);
                    }
                }
                set {
                    this[this.tableДоступВИнтернет.ПоColumn] = value;
                }
            }
            
            public bool IsПоNull() {
                return this.IsNull(this.tableДоступВИнтернет.ПоColumn);
            }
            
            public void SetПоNull() {
                this[this.tableДоступВИнтернет.ПоColumn] = System.Convert.DBNull;
            }
        }
        
        [System.Diagnostics.DebuggerStepThrough()]
        public class ДоступВИнтернетRowChangeEvent : EventArgs {
            
            private ДоступВИнтернетRow eventRow;
            
            private DataRowAction eventAction;
            
            public ДоступВИнтернетRowChangeEvent(ДоступВИнтернетRow row, DataRowAction action) {
                this.eventRow = row;
                this.eventAction = action;
            }
            
            public ДоступВИнтернетRow Row {
                get {
                    return this.eventRow;
                }
            }
            
            public DataRowAction Action {
                get {
                    return this.eventAction;
                }
            }
        }
    }
}