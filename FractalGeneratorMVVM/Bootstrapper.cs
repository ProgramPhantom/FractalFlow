﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Windows;
using FractalGeneratorMVVM.ViewModels;
using System.Data.SqlClient;
using FractalCore;
using System.Diagnostics;
using FractalCore.Painting;
using System.Windows.Media;
using FractalGeneratorMVVM.ViewModels.Models;
using FractalGeneratorMVVM.ViewModels.Models.Painters;

namespace FractalGeneratorMVVM
{
    public class Bootstrapper : BootstrapperBase
    {
        /// <summary>
        /// User interface shell (interacts with backend)
        /// </summary>
        public Kernel? Shell { get; set; }
        private struct FractalObjectPackage
        {
            public BindableCollection<FractalFrameViewModel> _fractalFrameViewModels;
            public BindableCollection<IteratorViewModel> _iteratorViewModels;
            public BindableCollection<IPainterViewModel> _painterViewModels;

            public FractalObjectPackage(BindableCollection<FractalFrameViewModel> fractalFrameViewModels,
            BindableCollection<IPainterViewModel> painterViewModels, BindableCollection<IteratorViewModel> iteratorViewModels)
            {
                _fractalFrameViewModels = fractalFrameViewModels;
                _iteratorViewModels = iteratorViewModels;
                _painterViewModels = painterViewModels;
            }
        }
        private string databaseName = "FractalFlowDB";
        public string AppDataFolder { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public string DatabaseLocation { get; }

        /// <summary>
        /// Bootstrapper 
        /// </summary>
        public Bootstrapper()
        {
            Initialize();
            DatabaseLocation = $"{AppDataFolder}\\FractalFlow\\{databaseName}";
        }

        #region Methods
        /// <summary>
        /// Called when the application starts by the Caliburn.Micro framework.
        /// Overridden from BootstrapperBase.
        /// </summary>
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            SplashScreen splash = new SplashScreen(@"\splash.png");
            splash.Show(false);

            // Load prior state from database
            try
            {
                FractalObjectPackage fromDb = LoadFromDatabase().Result;
                // Init shell with information from database.
                Shell = new Kernel(fromDb._fractalFrameViewModels, fromDb._painterViewModels, fromDb._iteratorViewModels);


            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                Shell = new Kernel();
            }


            splash.Close(TimeSpan.FromMilliseconds(200));

            Shell.ShowMainWindow();
        }
        /// <summary>
        /// Loads a <see cref="FractalObjectPackage"/> from the database to send to the shell
        /// </summary>
        /// <returns></returns>
        private async Task<FractalObjectPackage> LoadFromDatabase()
        {
            
            BindableCollection<FractalFrameViewModel> frames = new BindableCollection<FractalFrameViewModel>();
            BindableCollection<IteratorViewModel> iterators = new BindableCollection<IteratorViewModel>();
            BindableCollection<IPainterViewModel> painters = new BindableCollection<IPainterViewModel>();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(@$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={DatabaseLocation}.mdf;Integrated Security=True"))
                {
                    sqlConnection.Open();

                    InitializeDatabase(sqlConnection);

                    #region Read Fractal Frames
                    SqlCommand readFractalFrames = new SqlCommand("Select * From [FractalFrames]", sqlConnection);

                    SqlDataReader fractalFrameReader = readFractalFrames.ExecuteReader();

                    while (fractalFrameReader.Read())
                    {
                        float left = float.Parse(fractalFrameReader["left"].ToString()!);
                        float right = float.Parse(fractalFrameReader["right"].ToString()!);
                        float top = float.Parse(fractalFrameReader["top"].ToString()!);
                        float bottom = float.Parse(fractalFrameReader["bottom"].ToString()!);
                        uint iterations = uint.Parse(fractalFrameReader["iterations"].ToString()!);
                        int bail = int.Parse(fractalFrameReader["bail"].ToString()!);
                        string name = fractalFrameReader["name"].ToString()!;
                        string colourHex = fractalFrameReader["colour"].ToString()!;
                        Color color = (Color)ColorConverter.ConvertFromString(colourHex);

                        int number = int.Parse(fractalFrameReader["number"].ToString()!);

                        FractalFrame ff = new FractalFrame(left, right, top, bottom, name, iterations, bail);
                        frames.Add(new FractalFrameViewModel(number, ff, color, name));
                    }
                    await fractalFrameReader.CloseAsync();
                    #endregion

                    #region Read Iterators
                    SqlCommand readIterators = new SqlCommand("Select * From [Iterators]", sqlConnection);

                    SqlDataReader iteratorReader = readIterators.ExecuteReader();

                    int count = 1;
                    while (iteratorReader.Read())
                    {
                        string formulaString = iteratorReader["formulastring"].ToString()!;
                        string name = iteratorReader["name"].ToString()!;


                        iterators.Add(new IteratorViewModel(new BasicIterator(formulaString, name), count));
                        count++;
                    }
                    await iteratorReader.CloseAsync();
                    #endregion

                    #region Read Basic Painters
                    SqlCommand readBasicPainters = new SqlCommand("Select * From [BasicPainters]", sqlConnection);

                    SqlDataReader basicPainterReader = readBasicPainters.ExecuteReader();

                    count = 1;
                    while (basicPainterReader.Read())
                    {
                        string inSetColourHex = basicPainterReader["insetcolour"].ToString()!;
                        Color inSetColour = (Color)ColorConverter.ConvertFromString(inSetColourHex);

                        string mainColourHex = basicPainterReader["maincolour"].ToString()!;
                        Color mainColour = (Color)ColorConverter.ConvertFromString(mainColourHex);

                        bool type = bool.Parse(basicPainterReader["type"].ToString()!);
                        string name = basicPainterReader["name"].ToString()!;

                        if (type == false)
                        {
                            painters.Add(new BasicPainterDarkViewModel(new BasicPainterDark(name, mainColour, inSetColour), count, name));
                        }
                        else
                        {
                            painters.Add(new BasicPainterLightViewModel(new BasicPainterLight(name, mainColour, inSetColour), count, name));
                        }
                        count++;
                    }
                    await basicPainterReader.CloseAsync();
                    #endregion

                }

            }
            catch
            {
                throw new Exception("Could not load from database");
            }
            

            return new FractalObjectPackage(frames, painters, iterators);

        }
        /// <summary>
        /// Saves state of shell to the database
        /// </summary>
        /// <param name="k"></param>
        public  void SaveToDatabase(Kernel k)
        {
            BindableCollection<FractalFrameViewModel> frames = k.DefaultPage.FractalFrameStackVM.FractalFrameViewModels;
            BindableCollection<IteratorViewModel> iterators = k.DefaultPage.IteratorStackVM.IteratorViewModels;
            BindableCollection<IPainterViewModel> painters = k.DefaultPage.PainterStackVM.PainterViewModels;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(@$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""{DatabaseLocation}.mdf"";Integrated Security=True"))
                {
                    sqlConnection.Open();

                    SqlCommand delete;
                    SqlCommand insert;

                    // Delete all records (only remotely acceptable because there will not be many fields) 

                    delete = new SqlCommand("DELETE FROM FractalFrames;", sqlConnection);
                    delete.ExecuteNonQuery();
                    delete = new SqlCommand("DELETE FROM Iterators;", sqlConnection);
                    delete.ExecuteNonQuery();
                    delete = new SqlCommand("DELETE FROM BasicPainters", sqlConnection);
                    delete.ExecuteNonQuery();

                    #region Insert Fractal Frames
                    foreach (FractalFrameViewModel ffvm in frames)
                    {
                        insert = new SqlCommand(@$"INSERT INTO FractalFrames (Id, ""Name"", ""Left"", ""Right"", ""Bottom"", ""Top"", ""Iterations"", " +
                                                @$"""Bail"", ""Colour"", ""Number"") VALUES ('{ffvm.ID}', '{ffvm.Name}', {ffvm.FractalFrameModel.Left}, " +
                                                $"{ffvm.FractalFrameModel.Right}, {ffvm.FractalFrameModel.Bottom}, " +
                                                $"{ffvm.FractalFrameModel.Top}, {ffvm.FractalFrameModel.Iterations}, " +
                                                $"{ffvm.FractalFrameModel.Bail}, '{ffvm.Colour.Color}', {ffvm.Number});", sqlConnection);
                        insert.ExecuteNonQuery();
                    }

                    #endregion

                    #region Insert Iterators
                    foreach (IteratorViewModel iter in iterators)
                    {
                        insert = new SqlCommand($@"INSERT INTO Iterators (Id, ""FormulaString"", ""Name"") VALUES " +
                                                 $"('{iter.ID}', '{iter.FormulaString}', '{iter.Name}');", sqlConnection);
                        insert.ExecuteNonQuery();
                    }
                    #endregion

                    #region Insert Painters
                    foreach (IPainterViewModel painter in painters)
                    {
                        if (painter is BasicPainterBaseViewModelAbstract)
                        {
                            BasicPainterBase model = (BasicPainterBase)painter.PainterModel;
                            insert = new SqlCommand($@"INSERT INTO BasicPainters (Id, ""InSetColour"", ""MainColour"", ""Type"", ""Name"") VALUES " +
                                                    $@"('{painter.ID}', '{model.InSetColour}', '{model.MainColour}', '{((BasicPainterBaseViewModelAbstract)painter).Type}', " +
                                                    $@"'{painter.Name}');", sqlConnection);
                            insert.ExecuteNonQuery();
                        }
                    }
                    #endregion
                }

            }
            catch
            {
                throw new Exception("Save to database failed");
            }

        }
        /// <summary>
        /// Sets up the database if it is uninitialised
        /// </summary>
        /// <param name="sqlConnection"></param>
        private void InitializeDatabase(SqlConnection sqlConnection)
        {
            try
            {
                // CHECK IF TABLES EXIST
                int exists;

                // --- Fractal Frame View Models ---
                #region FractalFrames
                string checkFractalFrames = @"SELECT CASE WHEN OBJECT_ID('dbo.FractalFrames', 'U') IS NOT NULL THEN 1 ELSE 0 END";
                SqlCommand checkFF = new SqlCommand(checkFractalFrames, sqlConnection);
                exists = (int)checkFF.ExecuteScalar();

                if (exists == 0)
                {
                    // Create the FractalFrames table

                    string createFractalFramesTableString = @"CREATE TABLE [dbo].[FractalFrames]
                                                (
	                                                [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	                                                [Name] NVARCHAR(40) NULL,
                                                    [Left] FLOAT NOT NULL, 
                                                    [Right] FLOAT NOT NULL, 
                                                    [Bottom] FLOAT NOT NULL, 
                                                    [Top] FLOAT NOT NULL, 
                                                    [Iterations] INT NOT NULL, 
                                                    [Bail] INT NOT NULL,
                                                    [Colour] CHAR(9) NOT NULL,
                                                    [Number] INT NOT NULL
                                                )
                                                ";

                    SqlCommand createFractalFramesTable = new SqlCommand(createFractalFramesTableString, sqlConnection);
                    createFractalFramesTable.ExecuteNonQuery();
                }
                #endregion

                // --- Iterators --- 
                #region Iterators
                string checkIteratorsString = @"SELECT CASE WHEN OBJECT_ID('dbo.Iterators', 'U') IS NOT NULL THEN 1 ELSE 0 END";
                SqlCommand checkIterator = new SqlCommand(checkIteratorsString, sqlConnection);
                exists = (int)checkIterator.ExecuteScalar();

                if (exists == 0)
                {
                    string createIteratorsTableString = @"CREATE TABLE [dbo].[Iterators]
                                                        (
	                                                        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
                                                            [FormulaString] VARCHAR(50) NOT NULL, 
                                                            [Name] VARCHAR(50) NULL 
                                                        )";

                    SqlCommand createIteratorTable = new SqlCommand(@createIteratorsTableString, sqlConnection);
                    createIteratorTable.ExecuteNonQuery();
                }
                #endregion

                // --- Basic Painters --- 
                #region Basic Painters
                string checkBasicPaintersString = @"SELECT CASE WHEN OBJECT_ID('dbo.BasicPainters', 'U') IS NOT NULL THEN 1 ELSE 0 END";
                SqlCommand checkBasicPainters = new SqlCommand(checkBasicPaintersString, sqlConnection);
                exists = (int)checkBasicPainters.ExecuteScalar();

                if (exists == 0)
                {
                    string createBasicPainterTableString = @"CREATE TABLE [dbo].[BasicPainters]
                                                            (
	                                                            [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
                                                                [InSetColour] CHAR(9) NOT NULL, 
                                                                [MainColour] CHAR(9) NOT NULL, 
                                                                [Type] BIT NOT NULL, 
                                                                [Name] VARCHAR(50) NOT NULL
                                                            )
                                                            ";

                    SqlCommand createBasicPainterTable = new SqlCommand(@createBasicPainterTableString, sqlConnection);
                    createBasicPainterTable.ExecuteNonQuery();
                }
                #endregion
            } catch
            {
                throw new Exception("Could not initialise database");
            }

        }
        /// <summary>
        /// Called when application exits
        /// </summary>
        protected override void OnExit(object sender, EventArgs e)
        {
            try
            {
                SaveToDatabase(Shell!);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            


            base.OnExit(sender, e);

            
        }
        #endregion
    }
}
