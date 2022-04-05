namespace NewTheKStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("product")]
    public partial class product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public product()
        {
            cartinfoes = new HashSet<cartinfo>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int productID { get; set; }

        [StringLength(50)]
        public string subID { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        [StringLength(50)]
        public string processor { get; set; }

        public int? memory { get; set; }

        [StringLength(50)]
        public string storage { get; set; }

        public decimal? price { get; set; }

        [StringLength(50)]
        public string categorize { get; set; }

        [StringLength(50)]
        public string imgscr { get; set; }

        [StringLength(50)]
        public string manufacture { get; set; }

        [StringLength(50)]
        public string display { get; set; }

        [StringLength(50)]
        public string carouselmessage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cartinfo> cartinfoes { get; set; }
    }
}
