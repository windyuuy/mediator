using System.Collections.Generic;
using ArgList = System.Object;

namespace TMediator {
	public class Channel {
		public bool stopped = false;
		public object namesspace;
		public List<Subscriber> callbacks;
		public Dictionary<object, Channel> channels;
		public Channel parent;

		public Channel (object namesspace, Channel parent = null) {
			this.namesspace = namesspace;
			this.parent = parent;
			this.callbacks=new List<Subscriber>();
			this.channels=new Dictionary<object,Channel>();
		}

		public Subscriber addSubscriber (CallbackFn fn, Options options) {
			var self = this;
			var callback = new Subscriber (fn, options);
			var priority = self.callbacks.Count + 1;

			options = options != null?options : new Options ();

			if (options.priority >= 0 &&
				options.priority < priority) {
				priority = options.priority;
			}

			self.callbacks.Insert (priority, callback);

			return callback;
		}

		public Subscriber getSubscriber (int id) {
			var self = this;
			for (var i = 0; i < self.callbacks.Count; i++) {
				var callback = self.callbacks[i];
				if (callback.id == id) {
					return new Subscriber (i, callback);
				}
			}
			Subscriber sub = null;
			foreach (var key in self.channels) {
				var channel = self.channels[key];
				sub = channel.getSubscriber (id);
				if (sub != null) {
					break;
				}
			}
			return sub;
		}

		public void setPriority (int id, int priority) {
			var self = this;
			var callback = self.getSubscriber (id);

			if (callback.value != null) {
				self.callbacks.Remove (callback);
				self.callbacks.Insert (priority, callback.value);
			}
		}

		public Channel addChannel (object namesspace) {
			var self = this;
			self.channels[namesspace] = new Channel (namesspace, self);
			return self.channels[namesspace];
		}

		public bool hasChannel (object namesspace) {
			var self = this;
			return namesspace != null && self.channels[namesspace] != null && true;
		}

		public Channel getChannel (object namesspace) {
			var self = this;
			Channel channel=null;
			if(self.channels.ContainsKey(namesspace)){
				channel = self.channels[namesspace];
			}
			if (channel == null) {
				channel = self.addChannel (namesspace);
			}
			return channel;
		}

		public Subscriber removeSubscriber (int id) {
			var self = this;
			var callback = self.getSubscriber (id);

			if (callback != null && callback.value != null) {
				foreach (var key in self.channels) {
					var channel = self.channels[key];
					channel.removeSubscriber (id);
				}

				var ret = self.callbacks[callback.index];
				// self.callbacks.RemoveAt (callback.index);
				self.callbacks.Remove(ret);
				return ret;
			}
			return null;
		}

		public List<object> publish (List<object> result, ArgList arglist) {
			var self = this;
            var callbacksCopy = self.callbacks.ToArray();
			for (var i = 0; i < callbacksCopy.Length; i++) {
				var callback = callbacksCopy[i];

				if (callback.options.predicate == null || callback.options.predicate (arglist)) {
					var ret = callback.fn (arglist);
					object value =null;
					var continue1 = false;

					if(ret!=null){
						value = ret.value;
						continue1 = ret.continue1;
					}

					if (value != null) {
						result.Add (value);
					}
					if (!continue1) {
						return null;
					}
				}
			}

			if (self.parent != null) {
				return self.parent.publish (result, arglist);
			} else {
				return result;
			}
		}
	}
};