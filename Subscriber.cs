using ArgList = System.Object;

namespace TMediator {
	public class Subscriber {
		public int id;
		public Options options;
		public CallbackFn fn;
		public object channel;

		public int index;
		public Subscriber value;
		public Subscriber (CallbackFn fn, Options options) {
			this.fn=fn;
			this.options=options!=null?options:new Options();
			this.id = Utils.getUniqueId (this);
		}
		public Subscriber (int index, Subscriber callback) {
			this.index = index;
			this.value = callback;
		}
		public void update (Options options) {
			if (options != null) {
				var self = this;
				self.fn = options.fn != null?options.fn : self.fn;
				self.options = options.options != null?options.options : self.options;
			}
		}
	}
};