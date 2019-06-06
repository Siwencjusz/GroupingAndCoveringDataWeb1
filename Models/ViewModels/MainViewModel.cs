using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Core.Common.Interfaces;
using Core.Common.Interfaces.Excell;
using Core.Common.Interfaces.GroupingManager;
using Core.Common.Items;
using Core.Common.Items.MatrixFeatures;
using Models.Base;
using Prism.Commands;

namespace Models.ViewModels
{

    public class MainViewModel : BaseNotifyError
    {
        public ICommand CmdCancelComputeData { get; set; }
        public ICommand CmdComputeData { get; set; }
        public ICommand CmdReadFile { get; set; }
        public ICommand CmdExportToExcel { get; set; }
        public ICommand CmdExportTstToTxt { get; set; }
        public ICommand CmdExportTrnToTxt { get; set; }
        private const string BladPrzetwarzania = "Błąd przetwarzania";
        private const string NieprawidlowyPlik = "Nieprawidłowy plik";
        private string _currentPageString = "0/0";
        private int _isCurrentPage;
        private bool _isNextPageEnabled;
        private bool _isPreviousPageEnabled;
        private double? _high;
        private double? _low;
        private int _numberOfPages;
        private string _rawDataString;
        private double? _step;
        private bool _useStep;
        private bool _runIndicatorVisible = true;
        private bool _excelIndicatorVisible = true;
        private CoverResult _coverResult;
        private Matrix _coverMatrix;
        private string _fileName;
        private string[] _rawData;
        private double? _paramInput;
        private string _paramInputText = "Parametr grupowania";
        private bool _runButtonIndicatorVisible = true;


        private readonly IExcelWriter _excelWriter;
        private IEnumerable<IGroupingMethod> _methodsDelegates;
        private readonly ITxtExporter _txtExportert;
        private readonly IFileReaderProvider _fileReaderProvider;
        private readonly ICoverMatrixManager _coverMatrixManager;
        private readonly IFileChecker _fileChecker;
        private readonly IFileReader _fileReader;
        private readonly IOpenFileDialog _openFileDialog;

        public MainViewModel()
        {

        }


        public MainViewModel(
            IFileReaderProvider fileReaderProvider,
            ICoverMatrixManager coverMatrixManager,
            IExcelWriter excelWriter,
            IValidateService validateService,
            IGroupingManager groupingManager,
            ITxtExporter txtExporter,
            IFileChecker fileChecker,
            IFileReader fileReader,
            IOpenFileDialog openFileDialog) : base(validateService)
        {
            Progress = new ProgressBarModel();
            _txtExportert = txtExporter;
            _fileChecker = fileChecker;
            _fileReader = fileReader;
            _openFileDialog = openFileDialog;
            _fileReaderProvider = fileReaderProvider;
            _coverMatrixManager = coverMatrixManager;
            _excelWriter = excelWriter;
            RunIndicatorVisible = false;
            ExcelIndicatorVisible = false;
            CmdCancelComputeData = new DelegateCommand(async () => await CancelComputeData(), () => CancelEnabled);
            CmdComputeData = new DelegateCommand(async () => await ComputeData(), RunEnableCheck);
            CmdReadFile = new DelegateCommand(ReadFile, () => RunButtonIndicatorVisible);
            CmdExportToExcel = new DelegateCommand(async () => await ExportToExcel(), () => CanEnable);
            CmdExportTstToTxt = new DelegateCommand(async () => await ExportToTxt(CoverResult.TestMatrix, "_Obiekty_testowe"), () => CanEnable);
            CmdExportTrnToTxt = new DelegateCommand(async () => await ExportToTxt(CoverResult.DataMatrix, "_Obiekty_treningowe"), () => CanEnable);
            MethodsDelegates = groupingManager.GetGroupingMethods();
            SelectedMethod = MethodsDelegates.FirstOrDefault();
        }

        private bool CanEnable { get; set; }

        private async Task ExportToTxt(Matrix coverResultDataObjects, string suffix)
        {
            ExcelIndicatorVisible = true;
            Thread.CurrentThread.IsBackground = true;
            var result = await _txtExportert.ExportToTxt(coverResultDataObjects.DataTable, suffix);

            if (result.HasErrors())
            {
                ErrorBox(result);
            }

            ExcelIndicatorVisible = false;
        }

        private async Task ExportToExcel()
        {

            ExcelIndicatorVisible = true;
            Thread.CurrentThread.IsBackground = true;

            var result = await _excelWriter.ExportToExcel(_fileName, CoverSample, CoverSample.CoverResult.DataMatrix.DataTable, CoverResult.TestMatrix.DataTable);

            if (result.HasErrors())
            {
                ErrorBox(result);
            }

            ExcelIndicatorVisible = false;
        }

        private void ReadFile()
        {
            _openFileDialog.ShowDialog();
            var exist = _fileChecker.IsFileExsist(_openFileDialog.FileName);
            var extension = _fileChecker.IsFileHasAppriopriateExtension(_openFileDialog.FileName);

            if (!(extension && exist))
            {
                ErrorBox(NieprawidlowyPlik);
            }

            var fileName = _openFileDialog.SafeFileName;
            var rawData = _fileReader.GetFileContent(_openFileDialog.FileName);

            _fileName = fileName;
            _rawData = rawData;
            RawDataString = string.Join("\n", rawData);
            ((DelegateCommand)CmdComputeData).RaiseCanExecuteChanged();
        }

        private static MessageBoxResult ErrorBox<T>(Result<T> result)
        {
            return MessageBox.Show($"Błąd odczytu pliku - {result.Error}", BladPrzetwarzania, MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        private static MessageBoxResult ErrorBox(string error)
        {
            return MessageBox.Show($"Błąd odczytu pliku - {error}", BladPrzetwarzania, MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private async Task ComputeData()
        {
            ShowButtons(true, false, false);

            _cmdRunCancelationToken = new CancellationTokenSource();
            var token = _cmdRunCancelationToken.Token;
            token.ThrowIfCancellationRequested();

            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    var waitRun = Task.Run(() =>
                    {
                        while (!token.IsCancellationRequested)
                        {

                        }

                        token.ThrowIfCancellationRequested();
                    }, token);

                    await ComputeCoverSample(waitRun);
                }
                catch (ThreadAbortException e)
                {

                }
                catch (OperationCanceledException e)
                {

                }
                catch (Exception e)
                {

                }
                finally
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }, token, TaskCreationOptions.RunContinuationsAsynchronously, TaskScheduler.Default);

        }

        private async Task ComputeCoverSample(Task waitRun)
        {
            var fileData = _fileReaderProvider.ConvertFile(_fileName, _rawData);
            var low = LOW ?? 0;
            var high = HIGH ?? 2;
            var paramInput = ParamInput ?? 1;
            var step = UseStep ? Step.Value : 0;
            var method = SelectedMethod;



            var result = await
                    _coverMatrixManager.GetMatrix(fileData.Value, low, high, method,
                        paramInput, step, Progress, waitRun);
            
            if (result == null || waitRun.IsCompleted)
            {
                ShowButtons(false, true, false);
                return;
            }

            if (result.HasErrors())
            {
                ErrorBox(result);
                ShowButtons(false, true, false);
                return;
            }

            CoverSample = result.Value;
            result.Value.SHIGH = high;
            result.Value.SLOW = low;
            result.Value.STEP = step;
            result.Value.SelecteMethod = method.MethodName;
            result.Value.SelecteMethodParam = paramInput;
            CoverMatrixDataTable = CoverSample.CoverResult.DataMatrix.DataTable;
            Progress.Progress = 100;

            CanEnable = true;
            ShowButtons(false, true, true);
        }

        public CoverSampleResult CoverSample
        {
            get
            {
                return _coverSample;
            }
            set
            {
                _coverSample = value;
                if (value is null)
                {
                    CoverResult = null;
                }
                else
                {
                    CoverResult = value.CoverResult;
                }

            }
        }
        private CancellationTokenSource _cmdRunCancelationToken;
        private async Task CancelComputeData()
        {
            _cmdRunCancelationToken.Cancel();
            thread.Abort();

            await Task.Run(() =>
            {
                CancelEnabled = false;
                ((DelegateCommand)CmdCancelComputeData).RaiseCanExecuteChanged();

                CancelEnabled = true;
                ((DelegateCommand)CmdCancelComputeData).RaiseCanExecuteChanged();

                ShowButtons(false, true, false);
            });

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void ShowButtons(bool runIndicatorVisible, bool runButtonIndicatorVisible, bool canEnable)
        {
            ((DelegateCommand)CmdExportToExcel).RaiseCanExecuteChanged();
            ((DelegateCommand)CmdExportTrnToTxt).RaiseCanExecuteChanged();
            ((DelegateCommand)CmdExportTstToTxt).RaiseCanExecuteChanged();
            ((DelegateCommand)CmdReadFile).RaiseCanExecuteChanged();
            RunIndicatorVisible = runIndicatorVisible;
            RunButtonIndicatorVisible = runButtonIndicatorVisible;
            CanEnable = canEnable;
        }

        public string RawDataString
        {
            get { return _rawDataString; }
            set
            {
                _rawDataString = value;
                OnPropertyChanged(nameof(RawDataString));
            }
        }

        public double? HIGH
        {
            get
            {
                return _high;
            }
            set
            {
                _high = value;
                ValueRangeValidation(_high, 1, 2, nameof(HIGH));
                ((DelegateCommand)CmdComputeData).RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(HIGH));
            }
        }

        public double? LOW
        {
            get
            {
                return _low;
            }
            set
            {
                _low = value;
                ValueRangeValidation(_low, 0, 1, nameof(LOW));
                ((DelegateCommand)CmdComputeData).RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(LOW));
            }
        }

        public double? Step
        {
            get
            {
                return _step;
            }
            set
            {
                _step = value;
                ValueRangeValidation(_step, 0, 1, nameof(Step));
                ((DelegateCommand)CmdComputeData).RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(Step));
            }
        }
        public double? ParamInput
        {
            get
            {
                return _paramInput;
            }
            set
            {
                _paramInput = value;
                ValueRangeValidation(_paramInput, 0, 100, nameof(ParamInput));
                ((DelegateCommand)CmdComputeData).RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(ParamInput));
            }
        }

        public CoverResult CoverResult
        {
            get { return _coverResult; }
            set
            {
                _coverResult = value;
                if (value is null)
                {
                    CoverMatrix = null;
                }
                else
                {
                    CoverMatrix = _coverResult.DataMatrix;
                }

                OnPropertyChanged(nameof(CoverResult));
                ((DelegateCommand)CmdExportToExcel).RaiseCanExecuteChanged();
                ((DelegateCommand)CmdExportTrnToTxt).RaiseCanExecuteChanged();
                ((DelegateCommand)CmdExportTstToTxt).RaiseCanExecuteChanged();
            }
        }
        public Matrix CoverMatrix
        {
            get { return _coverMatrix; }
            set
            {
                _coverMatrix = value;
                OnPropertyChanged(nameof(CoverMatrix));
                OnPropertyChanged(nameof(CoverMatrixDataTable));
                OnPropertyChanged(nameof(CoverMatrixhigh));
                OnPropertyChanged(nameof(CoverMatrixlow));
            }
        }

        public bool UseStep
        {
            get { return _useStep; }
            set
            {
                _useStep = value;
                ((DelegateCommand)CmdComputeData).RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(UseStep));
            }
        }

        public bool IsNextPageEnabled
        {
            get { return _isNextPageEnabled; }
            set
            {
                _isNextPageEnabled = value;
                OnPropertyChanged(nameof(IsNextPageEnabled));
            }
        }

        public bool IsPreviousPageEnabled
        {
            get { return _isPreviousPageEnabled; }
            set
            {
                _isPreviousPageEnabled = value;
                OnPropertyChanged(nameof(IsPreviousPageEnabled));
            }
        }

        public int CurrentPage
        {
            get { return _isCurrentPage; }
            set
            {
                _isCurrentPage = value;
                CurrentPageString = CurrentPageString + "/" + NumberOfPages;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public int NumberOfPages
        {
            get { return _numberOfPages; }
            set
            {
                _numberOfPages = value;
                CurrentPageString = CurrentPageString + "/" + NumberOfPages;
                OnPropertyChanged(nameof(NumberOfPages));
            }
        }

        public string CurrentPageString
        {
            get { return _currentPageString; }
            set
            {
                _currentPageString = value;
                OnPropertyChanged(nameof(CurrentPageString));
            }
        }

        public string CoverMatrixhigh => CoverResult != null ? CoverResult.DataMatrix.HIGH.ToString(CultureInfo.InvariantCulture) : string.Empty;

        public string CoverMatrixlow => CoverResult != null ? CoverResult.DataMatrix.LOW.ToString(CultureInfo.InvariantCulture) : string.Empty;

        public bool RunIndicatorVisible
        {
            get { return _runIndicatorVisible; }
            set
            {
                _runIndicatorVisible = value;
                OnPropertyChanged(nameof(RunIndicatorVisible));
            }
        }
        public bool RunButtonIndicatorVisible
        {
            get { return _runButtonIndicatorVisible; }
            set
            {
                _runButtonIndicatorVisible = value;
                OnPropertyChanged(nameof(RunButtonIndicatorVisible));
            }
        }
        public string ParamInputText
        {
            get { return _paramInputText; }
            set
            {
                _paramInputText = value;
                OnPropertyChanged(nameof(ParamInputText));
            }
        }
        public bool ExcelIndicatorVisible
        {
            get { return _excelIndicatorVisible; }
            set
            {
                _excelIndicatorVisible = value;
                OnPropertyChanged(nameof(ExcelIndicatorVisible));
            }
        }

        public IEnumerable<IGroupingMethod> MethodsDelegates
        {
            get { return _methodsDelegates; }
            set
            {
                _methodsDelegates = value;
                OnPropertyChanged(nameof(MethodsDelegates));
            }
        }

        public IGroupingMethod SelectedMethod { get; set; }
        private bool _cancelEnabled = true;
        private DataTable _coverMatrixDataTable;
        private CoverSampleResult _coverSample;
        private Thread thread;

        public DataTable CoverMatrixDataTable
        {
            get { return _coverMatrixDataTable; }
            set
            {
                _coverMatrixDataTable = value;
                OnPropertyChanged(nameof(CoverMatrixDataTable));
            }
        }

        public ProgressBarModel Progress { get; set; }

        public bool CancelEnabled
        {
            get { return _cancelEnabled; }
            set
            {
                _cancelEnabled = value;
                OnPropertyChanged(nameof(CancelEnabled));
            }
        }

        public bool RunEnableCheck()
        {
            var highValueCheck = _high.HasValue && _high >= 1.0 && _high <= 2.0;
            var lowValueCheck = _low.HasValue && _low <= 1 && _low >= 0;
            var stepValueCheck = _step.HasValue && _useStep && _step > 0 && _step <= 1 || !_useStep;
            var dataValueCheck = !string.IsNullOrEmpty(RawDataString);
            var selectedMethod = SelectedMethod != null;
            var paramInput = ParamInput != null && _paramInput > 0;
            var isRunEnabled = highValueCheck &&
                           lowValueCheck &&
                           stepValueCheck &&
                           dataValueCheck &&
                           selectedMethod &&
                           paramInput;
            return isRunEnabled;
        }
    }
}