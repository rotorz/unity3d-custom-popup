// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using UnityEditor;
using UnityEngine;

namespace Rotorz.Games.UnityEditorExtensions
{
    /// <summary>
    /// A delegate that is invoked to populate a custom popup menu with options.
    /// </summary>
    /// <typeparam name="TValue">The type of value that is being selected.</typeparam>
    /// <param name="context">Context of the operation.</param>
    public delegate void CustomPopupPopulateDelegate<TValue>(ICustomPopupContext<TValue> context);


    /// <summary>
    /// Functions for adding custom popup controls to editor interfaces.
    /// </summary>
    public static class CustomPopupGUI
    {
        private static readonly string s_CommandNameValueChanged = "CustomPopupValueChanged";
        private static readonly int s_CustomMenuHint = typeof(CustomPopupGUI).GetHashCode();


        // Identifies the active control.
        private static EditorWindow s_ActiveControlWindow;
        private static int s_ActiveControlID;

        // New value that should be assigned to the active control.
        private static object s_ActiveControlNewValue;


        private static void SetActivePopupControl(int popupControlID, object value)
        {
            s_ActiveControlWindow = EditorWindow.focusedWindow;
            s_ActiveControlID = popupControlID;
            s_ActiveControlNewValue = null;
        }


        private static void OnNotifySelection<TValue>(TValue value)
        {
            s_ActiveControlNewValue = value;
            var valueChangedEvent = EditorGUIUtility.CommandEvent(s_CommandNameValueChanged);
            s_ActiveControlWindow.SendEvent(valueChangedEvent);
        }


        /// <summary>
        /// Populates and displays a custom popup.
        /// </summary>
        /// <example>
        /// <code language="csharp"><![CDATA[
        /// public static TValue MyCustomPopup<TValue>(
        ///     Rect position,
        ///     GUIContent valueLabel,
        ///     TValue value,
        ///     CustomPopupPopulateDelegate<TValue> populatePopupDelegate)
        /// {
        ///     int popupControlID = GUIUtility.GetControlID(FocusType.Passive);
        ///     if (EditorGUI.DropdownButton(position, valueLabel, FocusType.Keyboard)) {
        ///         CustomPopupGUI.DisplayPopup(position, popupControlID, value, populatePopupDelegate);
        ///     }
        ///     return CustomPopupGUI.HandlePopupValueSelection(popupControlID, value);
        /// }
        /// ]]></code>
        /// </example>
        /// <typeparam name="TValue">The type of value that is being selected.</typeparam>
        /// <param name="position">Absolute position of control on the GUI.</param>
        /// <param name="popupControlID">Control ID of the popup.</param>
        /// <param name="value">Current value selection.</param>
        /// <param name="populatePopupDelegate">Delegate that populates popup menu.</param>
        /// <seealso cref="HandlePopupValueSelection{TValue}(int, TValue)"/>
        public static void DisplayPopup<TValue>(Rect position, int popupControlID, TValue value, CustomPopupPopulateDelegate<TValue> populatePopupDelegate)
        {
            SetActivePopupControl(popupControlID, value);

            var popup = new CustomPopup<TValue>();
            var context = new PopupContext<TValue>(popup, value);
            populatePopupDelegate.Invoke(context);

            popup.ShowAsDropdown(position);
        }

        /// <summary>
        /// Handles selection of value from custom popup.
        /// </summary>
        /// <typeparam name="TValue">The type of value that is being selected.</typeparam>
        /// <param name="popupControlID">Control ID of the popup.</param>
        /// <param name="value">Current value selection.</param>
        /// <returns>
        /// The new value selection.
        /// </returns>
        /// <seealso cref="DisplayPopup{TValue}(Rect, int, TValue, CustomPopupPopulateDelegate{TValue})"/>
        public static TValue HandlePopupValueSelection<TValue>(int popupControlID, TValue value)
        {
            if (popupControlID == s_ActiveControlID && s_ActiveControlWindow == EditorWindow.focusedWindow) {
                if (Event.current.GetTypeForControl(popupControlID) == EventType.ExecuteCommand && Event.current.commandName == s_CommandNameValueChanged) {
                    try {
                        value = (TValue)s_ActiveControlNewValue;
                        GUI.changed = true;
                        Event.current.Use();
                    }
                    finally {
                        // Don't hang on to the selected value... avoid potentionally
                        // keeping unwanted objects alive.
                        s_ActiveControlNewValue = null;
                    }
                }
            }
            return value;
        }



        /// <summary>
        /// Custom popup control that populates and displays popup menu upon being pressed.
        /// </summary>
        /// <example>
        /// <code language="csharp"><![CDATA[
        /// public static int SortingLayerPopup(Rect position, GUIContent label, int sortingLayerID)
        /// {
        ///     string sortingLayerName = SortingLayer.IDToName(sortingLayerID);
        ///     using (var valueLabel = ControlContent.Basic(sortingLayerName)) {
        ///         return CustomPopupGUI.Popup(position, label, sortingLayerID, valueLabel, context => {
        ///             var popup = context.Popup;
        ///             foreach (var layer in SortingLayer.layers) {
        ///                 popup.AddCommand(layer.name)
        ///                     .Checked(() => layer.id == context.CurrentValue)
        ///                     .Action(() => context.NotifySelection(layer.id));
        ///             }
        ///         });
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        /// <typeparam name="TValue">The type of value that is being selected.</typeparam>
        /// <param name="position">Absolute position of control on the GUI.</param>
        /// <param name="label">Prefix label content for the control (specify <c>GUIContent.none</c>
        /// to omit label from drawn control).</param>
        /// <param name="value">Current value selection.</param>
        /// <param name="valueLabel">Label content representing teh current value selection.</param>
        /// <param name="populatePopupDelegate">Delegate that populates popup menu.</param>
        /// <param name="style">Style to use to draw the drop-down button.</param>
        /// <returns>
        /// The new value selection.
        /// </returns>
        public static TValue Popup<TValue>(Rect position, GUIContent label, TValue value, GUIContent valueLabel, CustomPopupPopulateDelegate<TValue> populatePopupDelegate, GUIStyle style)
        {
            int popupControlID = GUIUtility.GetControlID(s_CustomMenuHint, FocusType.Passive);

            bool hasPrefixLabel = (label != null && label != GUIContent.none);
            if (hasPrefixLabel) {
                position = EditorGUI.PrefixLabel(position, label);
            }

            if (EditorGUI.DropdownButton(position, valueLabel ?? GUIContent.none, FocusType.Keyboard, style)) {
                DisplayPopup(position, popupControlID, value, populatePopupDelegate);
            }

            return HandlePopupValueSelection(popupControlID, value);
        }

        /// <inheritdoc cref="Popup{TValue}(Rect, GUIContent, TValue, GUIContent, CustomPopupPopulateDelegate{TValue}, GUIStyle)"/>
        public static TValue Popup<TValue>(Rect position, GUIContent label, TValue value, GUIContent valueLabel, CustomPopupPopulateDelegate<TValue> populatePopupDelegate)
        {
            return Popup<TValue>(position, label, value, valueLabel, populatePopupDelegate, EditorStyles.popup);
        }



        /// <inheritdoc cref="Popup{TValue}(Rect, GUIContent, TValue, GUIContent, CustomPopupPopulateDelegate{TValue}, GUIStyle)"/>
        public static TValue Popup<TValue>(GUIContent label, TValue value, GUIContent valueLabel, CustomPopupPopulateDelegate<TValue> populatePopupDelegate, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect position = GUILayoutUtility.GetRect(valueLabel, style, options);
            return Popup<TValue>(position, label, value, valueLabel, populatePopupDelegate, style);
        }

        /// <inheritdoc cref="Popup{TValue}(Rect, GUIContent, TValue, GUIContent, CustomPopupPopulateDelegate{TValue}, GUIStyle)"/>
        public static TValue Popup<TValue>(GUIContent label, TValue value, GUIContent valueLabel, CustomPopupPopulateDelegate<TValue> populatePopupDelegate, params GUILayoutOption[] options)
        {
            return Popup<TValue>(label, value, valueLabel, populatePopupDelegate, EditorStyles.popup, options);
        }



        private sealed class PopupContext<TValue> : ICustomPopupContext<TValue>
        {
            public PopupContext(CustomPopup<TValue> popup, TValue currentValue)
            {
                this.Popup = popup;
                this.CurrentValue = currentValue;
            }


            /// <inheritdoc/>
            public CustomPopup<TValue> Popup { get; private set; }

            /// <inheritdoc/>
            public TValue CurrentValue { get; private set; }


            /// <inheritdoc/>
            public void NotifySelection(TValue newValue)
            {
                CustomPopupGUI.OnNotifySelection(newValue);
            }
        }
    }
}
