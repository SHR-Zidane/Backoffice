using System;
using System.IO;

namespace tounsiYoyo.Model
{
    public static class JsonPathResolver
    {
        public const string DefaultFileName = "COM-1510.json";

        /// <summary>
        /// Résout le chemin du fichier JSON de manière fiable.
        /// Cherche d'abord dans le répertoire de l'application (BaseDirectory),
        /// puis dans le répertoire de travail courant, puis dans les dossiers parents.
        /// </summary>
        public static string ResolvePath(string fileName = DefaultFileName)
        {
            // 1. Chercher dans le dossier de l'application (AppContext.BaseDirectory)
            string baseDir = AppContext.BaseDirectory;
            string candidate = Path.Combine(baseDir, fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            // 2. Chercher dans le dossier de travail courant (CurrentDirectory)
            string currentDir = Directory.GetCurrentDirectory();
            candidate = Path.Combine(currentDir, fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            // 3. Remonter l'arborescence depuis AppContext.BaseDirectory
            for (DirectoryInfo? dir = new DirectoryInfo(baseDir); dir != null; dir = dir.Parent)
            {
                candidate = Path.Combine(dir.FullName, fileName);
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                candidate = Path.Combine(dir.FullName, "tounsiYoyo", fileName);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            // 4. Remonter l'arborescence depuis Directory.GetCurrentDirectory()
            for (DirectoryInfo? dir = new DirectoryInfo(currentDir); dir != null; dir = dir.Parent)
            {
                candidate = Path.Combine(dir.FullName, fileName);
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                candidate = Path.Combine(dir.FullName, "tounsiYoyo", fileName);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            // Si introuvable, retourner le chemin par défaut dans le dossier de l'application
            return Path.Combine(baseDir, fileName);
        }
    }
}
