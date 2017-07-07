// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

namespace Rotorz.Games.UnityEditorExtensions
{
    /// <summary>
    /// Context of a <see cref="CustomPopup{TValue}"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of value that is being selected.</typeparam>
    public interface ICustomPopupContext<TValue>
    {
        /// <summary>
        /// The custom popup that is being built.
        /// </summary>
        CustomPopup<TValue> Popup { get; }

        /// <summary>
        /// Gets the value that is currently selected.
        /// </summary>
        TValue CurrentValue { get; }


        /// <summary>
        /// Notifies that a new value has been selected.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        void NotifySelection(TValue newValue);
    }
}
