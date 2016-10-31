using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL.Model
{
    public class TagInfo
    {
        private int id = -1;

        public int ID
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        private string name = string.Empty;
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }
    }
}
