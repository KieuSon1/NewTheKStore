namespace NewTheKStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cart")]
    public partial class cart
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public cart()
        {
            cartinfoes = new HashSet<cartinfo>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int cartID { get; set; }

        public DateTime orderdate { get; set; }

        [Column(TypeName = "date")]
        public DateTime deliverydate { get; set; }

        [StringLength(10)]
        public string location { get; set; }

        public int? userID { get; set; }

        public int? type { get; set; }

        public virtual account account { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cartinfo> cartinfoes { get; set; }
    }
}
