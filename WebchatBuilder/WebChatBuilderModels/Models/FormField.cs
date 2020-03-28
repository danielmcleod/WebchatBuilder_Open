using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebChatBuilderModels.Models
{
    public class FormField
    {
        [Key]
        public int FormFieldId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Label { get; set; }

        public string PlaceHolder { get; set; }

        public virtual FieldType FieldType { get; set; }

        public bool SendAsAttribute { get; set; }

        public bool AppendToCustomInfo { get; set; }

        public bool IsUserField { get; set; }

        public bool IsPhoneNumber { get; set; }

        public int Position { get; set; }

        public string CustomClasses { get; set; }

        public bool IsRequired { get; set; }

        public int MaxLength { get; set; }

        public virtual ICollection<FormOption> SelectOptions { get; set; }
    }

    public enum FieldType
    {
        Text,
        Password,
        Checkbox,
        Number,
        Date,
        Email,
        PhoneNumber,
        TextArea,
        Select,
        Profiles
    }
}
