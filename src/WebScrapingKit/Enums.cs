using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapingKit
{
    public enum HtmlDataGetterResult
    {
        Success = 0,
        FinalDataSuccess,
        LoadHtmlFailed,
        DataNotFound,
        GetInnerTextFailed,
        GetAttributeFailed
    }
}
