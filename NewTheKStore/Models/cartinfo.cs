namespace NewTheKStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cartinfo")]
    public partial class cartinfo
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int cartID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int productID { get; set; }

        [Column(TypeName = "money")]
        public decimal? price { get; set; }

        public int? quantity { get; set; }

        public virtual cart cart { get; set; }

        public virtual product product { get; set; }
    }
}
