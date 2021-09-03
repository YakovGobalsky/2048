using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2048 {
	public class PlayerPrefsValueInt {
		public event System.Action<int> onChanged;

		private int _value;
		private string storageKey;

		public PlayerPrefsValueInt(string key, int defaultValue) {
			storageKey = key;
			_value = PlayerPrefs.GetInt(storageKey, defaultValue);
		}

		public int Value {
			get {
				return _value;
			}
			set {
				_value = value;
				PlayerPrefs.SetInt(storageKey, value);
				onChanged?.Invoke(_value);
			}
		}

		public static implicit operator int (PlayerPrefsValueInt foo) => foo.Value;

	}
}