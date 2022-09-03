﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage;
using BeatLeader.Utils;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using HMUI;

namespace BeatLeader.Components
{
    [SerializeAutomatically("EditableElementsConfig")]
    internal class LayoutEditor : ReeUIComponentV2WithContainer
    {
        [SerializeAutomatically] private static Dictionary<string, bool> _objectsConfig = new();

        private List<EditableElement> _editableObjects = new();
        public bool EditMode { get; private set; }

        public void SetEditModeEnabled(bool enabled, bool applySettings = true)
        {
            if (enabled == EditMode) return;
            _editableObjects.ToList().ForEach(x => x.SetEnabled(enabled, applySettings));
            Content.gameObject.SetActive(enabled);
            EditMode = enabled;
        }
        public bool TryAddObject(EditableElement editable, bool loadConfig = true)
        {
            bool state = _objectsConfig.ContainsKey(editable.Name) && loadConfig ? _objectsConfig[editable.Name] : editable.Enabled;
            editable.OnStateChanged += SaveSettings; 

            bool result = false;
            if (!_editableObjects.Contains(editable))
            {
                _editableObjects.Add(editable);
                result = true;
            }
            return result;
        }
        public bool TryRemoveObject(string name)
        {
            if (EditMode) return false;
            var editable = _editableObjects.FirstOrDefault(x => x.Name == name);
            if (editable != null)
            {
                _editableObjects.Remove(editable);
                editable.Dispose();
                return true;
            }
            else return false;
        }

        private void SaveSettings(string name, bool value)
        {
            if (!_objectsConfig.TryAdd(name, value)) _objectsConfig[name] = value;
            AutomaticConfigTool.NotifyTypeChanged(GetType());
        }

        #region BSML Stuff

        [UIComponent("window")] private RectTransform _window;
        [UIComponent("handle")] private RectTransform _handle;

        protected override void OnInitialize()
        {
            Content.gameObject.SetActive(false);
            _window.gameObject.AddComponent<WindowView>().handle = _handle;
            _handle.gameObject.AddComponent<HighlightableView>().Init(Color.cyan, Color.yellow);
        }

        [UIAction("done-button-clicked")] private void OnDoneButtonClicked() => SetEditModeEnabled(false);
        [UIAction("cancel-button-clicked")] private void OnCancelButtonClicked() => SetEditModeEnabled(false, false);

        #endregion
    }
}
