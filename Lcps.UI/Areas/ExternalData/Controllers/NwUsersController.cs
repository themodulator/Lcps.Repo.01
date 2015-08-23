using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Lcps.DivisionDirectory.ExternalData;
using Lcps.UI.Models;

using Anvil;

using System.Linq.Dynamic;

namespace Lcps.UI.Areas.ExternalData.Controllers
{
    public class NwUsersController : Controller
    {
        ExternalDataContext _external = new ExternalDataContext();
        LcpsUiContext _lcpsDbContext = new LcpsUiContext();

        // GET: ExternalData/NwUsers
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult AddToDirectory(string key, string value)
        {
            string result = "OK";

            List<NWUser> l = _external.NWUsers.Where(key, new object[] { value }).ToList();

            if (l.Count() == 0)
                result = (new ExceptionCollector(string.Format("{0} = {1} was not found in NWUSers table", key, value))).ToUL();
            else
            {
                try
                {
                    NwUserCandidate c = new NwUserCandidate(l[0]);
                    c.AddToDirectory();
                }
                catch(Exception ex)
                {
                    result = (new ExceptionCollector(ex).ToUL());
                }
            }

            return Content(result);

        }
       
    }
}