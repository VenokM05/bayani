// BAYANI — Dialogue data: fully data-driven story lines (phasing rule: values in assets).
// Authoring happens via the Editor builders (Assets/Data/Story/*.asset); runtime is read-only.

using System;
using UnityEngine;

namespace Bayani.Story
{
    [Serializable]
    public class DialogueLine
    {
        public string speaker = "";       // "" or "NARRATION" renders as centered italic voice-over
        public string text = "";
    }

    [CreateAssetMenu(menuName = "BAYANI/Dialogue Sequence")]
    public class DialogueAsset : ScriptableObject
    {
        public DialogueLine[] lines;
    }
}
