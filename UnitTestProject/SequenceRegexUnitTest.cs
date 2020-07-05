using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace UnitTestProject
{
    [TestClass]
    public class SequenceRegexUnitTest
    {
        [TestMethod]
        public void Test()
        {
            string nextVal = Regex.Match("nextval('\"MyTable1_Id_seq_9336\"'::regclass)", "\\'\\\"[\\d\\W\\w_]+\\\"\\'", RegexOptions.ExplicitCapture)?.Value;
            Debug.WriteLine(nextVal);
            nextVal = nextVal.Replace("\'", string.Empty);
            Debug.WriteLine(nextVal);
        }
    }
}
