using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarPDemo.Models
{
    class AccountModel
    {
        private Int32 _ID;
        private String _UserName;
        private String _UName;
        private String _UPswd;
        private Int32 _ULevel;
        private Int32 _UState;

        public Int32 UId
        {
            get { return this._ID; }
            set { this._ID = value; }
        }

        public String UserName
        {
            get { return this._UserName; }
            set { this._UserName = value; }
        }

        public String UName
        {
            get { return this._UName; }
            set { this._UName = value; }
        }

        public String UPass
        {
            get { return this._UPswd; }
            set { this._UPswd = value; }
        }

        public Int32 ULevel
        {
            get { return this._ULevel; }
            set { this._ULevel = value; }
        }

        public Int32 UState
        {
            get { return this._UState; }
            set { this._UState = value; }
        }
    }
}
