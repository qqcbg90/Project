//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace KingspModel.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class MESSAGE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MESSAGE()
        {
            this.MESSAGE_LOG = new HashSet<MESSAGE_LOG>();
            this.ATTACHMENT = new HashSet<ATTACHMENT>();
        }
    
        public string ID { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
        public string CREATER { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string UPDATER { get; set; }
        public string MSG_TYPE { get; set; }
        public byte ENABLE { get; set; }
        public string NODE_ID { get; set; }
        public string CONTENT { get; set; }
        public string CONTENT1 { get; set; }
        public string CONTENT2 { get; set; }
        public string CONTENT3 { get; set; }
        public string CONTENT4 { get; set; }
        public string CONTENT5 { get; set; }
        public string CONTENT6 { get; set; }
        public string CONTENT7 { get; set; }
        public string CONTENT8 { get; set; }
        public string CONTENT9 { get; set; }
        public string CONTENT10 { get; set; }
        public Nullable<System.DateTime> DATETIME1 { get; set; }
        public Nullable<System.DateTime> DATETIME2 { get; set; }
        public Nullable<System.DateTime> DATETIME3 { get; set; }
        public Nullable<System.DateTime> DATETIME4 { get; set; }
        public Nullable<System.DateTime> DATETIME5 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MESSAGE_LOG> MESSAGE_LOG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTACHMENT> ATTACHMENT { get; set; }
    }
}