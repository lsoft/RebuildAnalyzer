﻿using Microsoft.Build.Construction;
using RebuildAnalyzer.Analyzer.Solution.Project;
using RebuildAnalyzer.Helper;

namespace RebuildAnalyzer.Analyzer.Solution
{
    public sealed class SlnAnalyzer
    {
        public IReadOnlyList<string> SkippedProjects { get; }
        public string RootFolder { get; }
        public string SlnRelativeFilePath { get; }

        public string SlnFullFilePath => Path.Combine(RootFolder, SlnRelativeFilePath);

        public SlnAnalyzer(
            IReadOnlyList<string> skippedProjects,
            string rootFolder,
            string slnRelativeFilePath
            )
        {
            if (skippedProjects is null)
            {
                throw new ArgumentNullException(nameof(skippedProjects));
            }

            if (rootFolder is null)
            {
                throw new ArgumentNullException(nameof(rootFolder));
            }

            if (slnRelativeFilePath is null)
            {
                throw new ArgumentNullException(nameof(slnRelativeFilePath));
            }
            SkippedProjects = skippedProjects;
            RootFolder = rootFolder;
            SlnRelativeFilePath = slnRelativeFilePath;
        }

        public bool IsAffected(Changeset changeset)
        {
            if (changeset.Contains(SlnRelativeFilePath))
            {
                //сам slnf изменился
                return true;
            }

            var sln = Microsoft.Build.Construction.SolutionFile.Parse(SlnFullFilePath);

            if (sln is null)
            {
                //sln не найден
                return false;
            }

            if (changeset.Contains(SlnRelativeFilePath))
            {
                //сам sln изменился
                return true;
            }

            var slnFolder = new FileInfo(SlnFullFilePath).Directory.FullName;

            var result = false;

            //параллелизуем для ускорения работы
            Parallel.ForEach(sln.ProjectsInOrder, new ParallelOptions { MaxDegreeOfParallelism = Math.Max(2, Environment.ProcessorCount - 2) }, (projectInSolution, state) =>
            {
                if(projectInSolution.ProjectType.In(SolutionProjectType.SolutionFolder))
                {
                    return;
                }

                var relativeProjectFilePath = projectInSolution.RelativePath;

                if (SkippedProjects.Contains(relativeProjectFilePath))
                {
                    //этот проект проверять не надо
                    return;
                }

                var projectAnalyzer = ProjectAnalyzerFactory.TryCreate(
                    slnFolder,
                    relativeProjectFilePath
                    );
                if (projectAnalyzer is null)
                {
                    //неизвестный проект, поэтому, на всякий случай, сообщаем, что sln изменился
                    result = true;
                    state.Break();
                    return;
                }

                if (projectAnalyzer.IsAffected(changeset))
                {
                    //проект изменился, дальше крутить смысла нет
                    result = true;
                    state.Break();
                    return;
                }
            });

            return result;
        }

    }
}
