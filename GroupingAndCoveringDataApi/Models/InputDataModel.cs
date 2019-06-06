using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GroupingAndCoveringDataApi.Models
{
    public class InputDataModel
    {
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public double Low { get; set; }
        [Required]
        public double High { get; set; }
        [Required]
        public string MethodName { get; set; }
        [Required]
        public double ParamInput { get; set; }
        [Required]
        public Guid CancelTokenGuid { get; set; }
        public string Step { get; set; }
    }
}
