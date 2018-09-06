using System;
using System.Collections.Generic;
using ArgList = System.Object;

namespace TMediator {

	public class Mediator {
		protected Channel channel;

		public Mediator(){
			channel=new Channel ("root");
		}

		public Channel getChannel (string[] channelNamespace) {
			var self = this;
			Channel channel = self.channel;

			for (var i = 0; i < channelNamespace.Length; i++) {
				channel = channel.getChannel (channelNamespace[i]);
			}

			return channel;
		}
		public Subscriber subscribe (string[] channelNamespace, CallbackFn fn, Options options=null) {
			var self = this;
			return self.getChannel (channelNamespace).addSubscriber (fn, options);
		}
		public Subscriber subscribe (string[] channelNamespace, CallbackFn2 fn, Options options=null) {
			var self = this;
			return self.getChannel (channelNamespace).addSubscriber ((arglist)=>{
				fn(arglist);return new CallbackResult();
			}, options);
		}
		public Subscriber subscribe (string channelname, CallbackFn2 fn, Options options=null) {
			return this.subscribe(new string[]{channelname},fn,options);
		}

		public Subscriber once(string channelname,CallbackFn fn,Options options=null){
			var id=0;
			var sub=this.subscribe(new string[]{channelname},(e)=>{
				this.removeSubscriber(id,new string[]{channelname});

				fn(e);
			},options);
			id=sub.id;
			return sub;
		}
		public Subscriber once(string channelname,CallbackFn2 fn,Options options=null){
			return this.once(channelname,(arglist)=>{
				fn(arglist);return new CallbackResult();
			},options);
		}

		public Subscriber getSubscriber (int id, string[] channelNamespace) {
			var self = this;
			return self.getChannel (channelNamespace).getSubscriber (id);
		}

		public Subscriber removeSubscriber (int id, string[] channelNamespace) {
			var self = this;
			return self.getChannel (channelNamespace).removeSubscriber (id);
		}

		public List<object> publish (string[] channelNamespace, ArgList arglist) {
			var self = this;
			return self.getChannel (channelNamespace).publish (new List<object> (), arglist);
		}
		public List<object> publish (string name, ArgList arglist) {
			return this.publish(new string[]{name},arglist);
		}
	}
};