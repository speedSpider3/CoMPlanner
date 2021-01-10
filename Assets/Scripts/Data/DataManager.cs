﻿using CasePlanner.UI;
using UnityEngine;
using SimpleFileBrowser;
using System.Collections;
using CasePlanner.Data;
using CasePlanner.Data.Notes;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Common.UI;

public class DataManager : MonoBehaviour {
    [SerializeField] private StickyNoteCreator noteCreator = null;
    [SerializeField] private PinConnector pinConnector = null;
    [SerializeField] private GameObject settingsMenu = null;

    private string currentCaseName = null;
    private bool settingsOpen = false;

	private void Start() {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Case Files", ".case"));
	}

	private void Update() {
        bool saveKey, loadKey;

        saveKey = Input.GetKeyDown(KeyCode.S) | Input.GetKeyDown(KeyCode.K);
        loadKey = Input.GetKeyDown(KeyCode.O) | Input.GetKeyDown(KeyCode.L);

        if (Input.GetKeyDown(KeyCode.Escape)) {
            ToggleSettings();
		}

        if (Input.GetKey(KeyCode.LeftControl)) {
            if (saveKey) {
                StartCoroutine(nameof(ShowSaveDialogAsync));
			} else if (loadKey) {
                StartCoroutine(nameof(ShowLoadDialogAsync));
            }
        } 
    }

    public List<StickyNote> GetNoteList() {
        return GameObject.FindGameObjectsWithTag("Note").Select(o => o.GetComponent<StickyNote>()).OrderBy(n => n.ID).ToList();
    }

    public void CreateDeserializedNote(StickyNote.SerializeNote serializedNote) {
        StickyNote note = noteCreator.CreateNote();
        note.Deserialize(serializedNote);
    }

    public void CreateDeserializedEdges(Yarn.Edge[] edges) {
        foreach (Yarn.Edge edge in edges) {
            if (edge is null || (edge.a == edge.b)) {
                continue;
            }

            pinConnector.A = GetFromID(edge.a).Pin;
            pinConnector.B = GetFromID(edge.b).Pin;
            pinConnector.Connect();
        }
    }

    public void Quit() {
        Application.Quit();
	}

    private IEnumerator ShowSaveDialogAsync() {
        string prevPath = PlayerPrefs.GetString("PrevPath", null);
        PanCamera.CamEnabled = false;
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, prevPath, currentCaseName);
        PanCamera.CamEnabled = true;

        if (FileBrowser.Success) {
            string path = FileBrowser.Result[0];
            int indexOfLastSeparator = path.LastIndexOf(Path.DirectorySeparatorChar) + 1;
            PlayerPrefs.SetString("PrevPath", path.Substring(0, indexOfLastSeparator));
            currentCaseName = path.Substring(indexOfLastSeparator);
            Serializer.Serialize(this, path);
        }
	}

    private IEnumerator ShowLoadDialogAsync() {
        string prevPath = PlayerPrefs.GetString("PrevPath", null);
        PanCamera.CamEnabled = false;
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, prevPath, currentCaseName);
        PanCamera.CamEnabled = true;

        if (FileBrowser.Success) {
            string path = FileBrowser.Result[0];
            int indexOfLastSeparator = path.LastIndexOf(Path.DirectorySeparatorChar) + 1;
            PlayerPrefs.SetString("PrevPath", path.Substring(0, indexOfLastSeparator));
            currentCaseName = path.Substring(indexOfLastSeparator);
            Serializer.Deserialize(this, path);
        }
	}

    private StickyNote GetFromID(int id) {
        return GetNoteList().Where(n => n.ID == id).FirstOrDefault();
    }

    private void ToggleSettings() {
        PanCamera.CamEnabled = settingsOpen;
        settingsOpen = !settingsOpen;
        settingsMenu.SetActive(settingsOpen);
    }
}