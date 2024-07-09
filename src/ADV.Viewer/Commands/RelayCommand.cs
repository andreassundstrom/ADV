// <copyright file="RelayCommand.cs" company="Andreas Sundström">
// Copyright (c) Andreas Sundström. All rights reserved.
// </copyright>

using System.Windows.Input;

namespace ADV.Viewer.Commands
{
    /// <summary>
    /// A default command to relay to.
    /// </summary>
    /// <param name="executeMethod">The method to invoke.</param>
    /// <param name="executePredicate">Test for if the method can be invoked.</param>
#pragma warning disable SA1009 // Closing parenthesis should be spaced correctly
    internal class RelayCommand(Action<object?> executeMethod, Predicate<object?> executePredicate) : ICommand
#pragma warning restore SA1009 // Closing parenthesis should be spaced correctly
    {
        /// <summary>
        /// Subscribe to change in can execute.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// If the element can be executed.
        /// </summary>
        /// <param name="parameter">Optional input parameter.</param>
        /// <returns>True if command can be executed.</returns>
        public bool CanExecute(object? parameter)
        {
            return executePredicate.Invoke(parameter);
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="parameter">Optional parameters.</param>
        public void Execute(object? parameter)
        {
            executeMethod.Invoke(parameter);
        }
    }
}
