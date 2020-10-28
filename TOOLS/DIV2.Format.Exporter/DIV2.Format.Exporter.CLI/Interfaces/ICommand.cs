namespace DIV2.Format.Exporter.CLI.Interfaces
{
    public interface ICommand
    {
        #region Methods & Functions
        /// <summary>
        /// Run the command.
        /// </summary>
        /// <param name="args">Arguments passed to command.</param>
        /// <returns>Returns a result code.</returns>
        int Run(params string[] args);

        /// <summary>
        /// Print the help information.
        /// </summary>
        void PrintHelp();
        #endregion
    }
}
