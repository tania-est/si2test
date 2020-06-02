using si2.dal.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace si2.dal.Entities
{
    [Table("Vehicle")]
    public class Vehicle : Si2BaseDataEntity<Guid>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Tag { get; set; }


        //Add comment 1 - Tania




        /// Add comment 2 -Taniaaa

        //comment 2

    }
}
