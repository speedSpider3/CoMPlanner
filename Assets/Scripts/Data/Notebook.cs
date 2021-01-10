﻿using UnityEngine;
using TMPro;

namespace CasePlanner.Data.Notes {
	public class Notebook : MonoBehaviour {
		[SerializeField] private TMP_InputField inputField = null;

		public TMP_InputField InputField { get => inputField; set => inputField = value; }
	}
}
