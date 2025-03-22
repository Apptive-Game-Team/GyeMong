using System;
using UnityEngine;

namespace Art_Resources.Simple_2D_Platformer_BE2.ReadMe.Scripts
{
	public class ReadmeBE2 : ScriptableObject {
		public Texture2D icon;
		public string title;
		public Section[] sections;
		public bool loadedLayout;
	
		[Serializable]
		public class Section {
			public string heading, text, linkText, url;
		}
	}
}
