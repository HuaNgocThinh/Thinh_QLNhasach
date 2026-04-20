using System.Collections.Generic;
using Thinh_QLNhasach.Database;
using Thinh_QLNhasach.Models;

namespace Thinh_QLNhasach.Controllers
{
    public class SachController
    {
        SachDB db = new SachDB();

        public List<Sach> GetAll()
        {
            return db.GetAll();
        }

        public void Add(Sach s)
        {
            db.Insert(s);
        }
    }
}