using System.Reflection;

namespace F10.StreamDeckIntegration {
	internal class StreamDeckButtonData {

		public string Id { get; }

		public object Target { get; }

		public MemberInfo Member { get; }

		public StreamDeckButtonData(object target, MemberInfo member, string id = null) {
			if (id == null) {
				id = member.Name;
			}

			Id = id;
			Target = target;
			Member = member;
		}

	}
}