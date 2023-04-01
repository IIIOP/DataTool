using System;
using DataToolInterface.Auxiliary;

namespace DataToolInterface.Format.Config
{
    public enum DataToolEnum
    {
        DataTool,
    }
    public class DataToolAttribute : Attribute
    {
        [Require]
        public string project { get; set; }
        [Require]
        public string name { get; set; }
        [Require]
        public string version { get; set; }
        [Require]
        public string author { get; set; }
    }
}
