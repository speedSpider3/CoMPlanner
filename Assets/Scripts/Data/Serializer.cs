﻿using UnityEngine;

using CasePlanner.Data.Notes;
using System.IO;
using System.Collections.Generic;

namespace CasePlanner.Data {
    [System.Serializable]
    public static class Serializer {
        [System.Serializable]
        public class SerializeBoard {
            public StickyNote.SerializeNote[] notes;
            public Yarn.Edge[] edges;
        }

        public static void Serialize(DataManager manager, string casePath) {
			SerializeBoard board = new SerializeBoard();

			SerializeNotes(manager.Notes, ref board);

			string output = JsonUtility.ToJson(board);
			File.WriteAllText(casePath, output);
		}

        public static void Deserialize(DataManager manager, string casePath) {
			SerializeBoard board = (SerializeBoard) JsonUtility.FromJson(File.ReadAllText(casePath), typeof(SerializeBoard));

			foreach (StickyNote.SerializeNote note in board.notes) {
				manager.CreateDeserializedNote(note);
			}

			manager.CreateDeserializedEdges(board.edges);
		}

        private static void SerializeNotes(List<StickyNote> notes, ref SerializeBoard board) {
            List<StickyNote.SerializeNote> serializedNotes = new List<StickyNote.SerializeNote>();
            List<Yarn.Edge> serializedEdges = new List<Yarn.Edge>();

            foreach (StickyNote note in notes) {
                serializedNotes.Add(note.Serialized());

                foreach (Yarn edge in note.Edges) {
                    Yarn.Edge e = edge.Serialized();

                    if (!serializedEdges.Contains(e)) {
                        serializedEdges.Add(e);
                    }
                }
            }

            board.notes = serializedNotes.ToArray();
            board.edges = serializedEdges.ToArray();
        }
    }
}