// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System.Collections.Generic;

namespace Rotorz.Games.UnityEditorExtensions
{
    /// <summary>
    /// Essentially the model of a custom popup menu.
    /// </summary>
    /// <typeparam name="TValue">The type of value that is being selected.</typeparam>
    public class CustomPopup<TValue> : EditorMenu
    {
        /// <summary>
        /// Adds an option to the <see cref="CustomPopup{TValue}"/>.
        /// </summary>
        /// <example>
        /// <para>Add command with a straightforward action:</para>
        /// <code language="csharp"><![CDATA[
        /// value = CustomPopupGUI.Popup(label, value, valueLabel, context => {
        ///     var popup = context.Popup;
        ///     popup.AddOption("First", context, 1);
        ///     popup.AddOption("Second", context, 2);
        ///     popup.AddOption("Third", context, 3);
        /// });
        /// ]]></code>
        /// </example>
        /// <param name="fullPath">Full path of the menu option.</param>
        /// <param name="context">Custom popup context.</param>
        /// <param name="value">The option value.</param>
        /// <returns>
        /// Fluid style API to further define the new command entry.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="fullPath"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="fullPath"/> is an empty string or starts with a slash.
        /// </exception>
        /// <seealso cref="EditorMenu.AddCommand(string)"/>
        /// <seealso cref="EditorMenu.AddSeparator(string)"/>
        public virtual EditorMenuCommandBinder AddOption(string fullPath, ICustomPopupContext<TValue> context, TValue value)
        {
            return this.AddCommand(fullPath)
                .Checked(() => EqualityComparer<TValue>.Default.Equals(value, context.CurrentValue))
                .Action(() => context.NotifySelection(value));
        }
    }
}
