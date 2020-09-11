﻿namespace RetroUnity.Utility {
    /// <summary>
    /// Interface for loading DLL's and their functions.
    /// </summary>
    public interface IDLLHandler {

        /// <summary>
        /// What core to load.
        /// </summary>
        /// <param name="dllPath">The full path to the core DLL.</param>
        /// <returns>Returns true if loading was successful.</returns>
        bool LoadCore(string dllPath);

        void UnloadCore();

        /// <summary>
        /// Get a method from the loaded DLL.
        /// </summary>
        /// <typeparam name="T">The method delegate type.</typeparam>
        /// <param name="functionName">Name of the method.</param>
        /// <returns>The method delegate.</returns>
        T GetMethod<T>(string functionName) where T : class;

    }
}
