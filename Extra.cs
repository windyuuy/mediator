using System;
using System.Collections.Generic;
using ArgList = System.Object;

namespace TMediator {
	public class CallbackResult {
		public bool continue1=false;
		public object value=new object();
	}
	public delegate bool CallbackPredicate (ArgList x);
	public delegate CallbackResult CallbackFn (ArgList arglist);
	public delegate void CallbackFn2 (ArgList arglist);

	public class Utils {
		public static int getUniqueId (object obj) {
			return obj.GetHashCode ();
		}
	}

	public class Options {
        public Options()
        {

        }
        public Options(CallbackPredicate predicate, int priority = 0)
        {
            this.predicate = predicate;
            this.priority = priority;
        }
		public int priority;
		public CallbackPredicate predicate;
		public CallbackFn fn;
		public Options options;
	}
}