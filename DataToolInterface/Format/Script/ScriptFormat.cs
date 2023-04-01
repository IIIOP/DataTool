using System;

namespace DataToolInterface.Format.Script
{
    /// <summary>
    /// Read input ,Read Output
    /// </summary>
    public class TestCaseAttribute:Attribute
    {
        public string id { get; set; }
        public string author { get; set; }
        public string describe { get; set; }

        public bool Result;
    }
    
    /// <summary>
    /// Read input
    /// </summary>
    public class CheckConsistencyAttribute:Attribute
    {
        public string id { get; set; }
        public string author { get; set; }
        public string describe { get; set; }
    }
    
    
    /// <summary>
    /// Read input
    /// </summary>
    public class GenerateOutputAttribute:Attribute
    {
        public string id { get; set; }
        public string author { get; set; }
        public string describe { get; set; }
    }
    

}