//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Blog_Project.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class COURS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public COURS()
        {
            this.EVALUATIONs = new HashSet<EVALUATION>();
        }
    
        public int ID { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<System.DateTime> MODIFIED_AT { get; set; }
        public Nullable<int> CREATED_BY { get; set; }
        public Nullable<int> MODIFIED_BY { get; set; }
        public Nullable<bool> INVALIDATED { get; set; }
        public Nullable<int> PROFESSOR_ID { get; set; }
        public string Title { get; set; }
        public Nullable<System.DateTime> Exam_Date { get; set; }
        public string Evaluated_Time { get; set; }
        public Nullable<int> isEvaluated { get; set; }
        public Nullable<int> Review { get; set; }
        public Nullable<int> Retake { get; set; }
    
        public virtual Users_Table Users_Table { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EVALUATION> EVALUATIONs { get; set; }
    }
}
