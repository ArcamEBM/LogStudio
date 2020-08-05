using System;
using System.Collections.Generic;
using System.Linq;

namespace LogStudio.Data
{
    public static class BuildHelpers
    {
        /// <summary>
        /// Get start time of build.
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        /// <remarks>
        /// Returns the first occurance of one of the following:
        /// 1. Play is pressed
        /// 2. ProcessManagerState is Running
        /// 3. InternalProcessManagerState is Running
        /// 4. Temperature has a logged value above 600 deg.
        /// </remarks>
        public static DateTime GetBuildStart(IItemDatabase database)
        {
            LogRowData startProcess = database.GetFirstItem("Process.ProcessManager.StartProcess", x => x.Value == "True");
            LogRowData processmanagerState = database.GetFirstItem("Process.ProcessManager.ProcessManagerState", x => x.Value == "Running");
            LogRowData internalProcessmanagerState = database.GetFirstItem("Process.ProcessManager.InternalProcessManagerState", x => x.Value == "Running");

            if (startProcess == null && processmanagerState == null && internalProcessmanagerState == null)
                throw new ApplicationException("Start of process not found.");

            List<LogRowData> data = new List<LogRowData>() { startProcess, processmanagerState, internalProcessmanagerState };
            DateTime result = data.Where(t => t != null).Min(p => p.TimeStamp);

            return result;
        }

        /// <summary>
        /// Get the end time of the build.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="buildEnd"></param>
        /// <param name="completed"></param>
        /// <remarks>
        /// Will return the timestamp of the following in prioritized order:
        /// 1. Build is done, completed is set to true
        /// 2. Build is crashed.
        /// 3. Time of the last action in the log file.
        /// 4. Time of the last task.
        /// </remarks>
        public static void GetBuildEnd(IItemDatabase db, out DateTime buildEnd, out bool completed)
        {
            LogRowDataPoint buildDone = db.GetLastItemDP("Process.ProcessManager.BuildDone", x => x.Value == 1);
            LogRowDataPoint buildCrashed = db.GetLastItemDP("Process.ProcessManager.BuildCrashed", x => x.Value == 1);

            LogRowDataPoint stopProcess = db.GetLastItemDP("Process.ProcessManager.StopProcess", x => x.Value == 1);
            LogRowDataPoint lastTask = db.GetLastItemDP("Process.ProcessManager.Task");

            LogRowDataPoint lastMeltAction = db.GetLastItemDP("Process.ProcessManager.Action");

            completed = false;

            if (buildDone != null && buildDone.Value == 1)
            {
                buildEnd = buildDone.TimeStamp;
                completed = true;
            }
            else if (buildCrashed != null && buildCrashed.Value == 1)
            {
                buildEnd = buildCrashed.TimeStamp;
            }
            else if (lastMeltAction != null)
            {
                buildEnd = lastMeltAction.TimeStamp;
            }
            else if (stopProcess != null)
            {
                buildEnd = stopProcess.TimeStamp;
            }
            else if (lastTask != null)
            {
                buildEnd = lastTask.TimeStamp;
            }
            else
                throw new ApplicationException("End of build not found.");
        }

        /// <summary>
        /// Get the time from the build starts to the heating ends
        /// </summary>
        /// <param name="db"></param>
        /// <param name="startTime">Time when build is started</param>
        /// <returns></returns>
        public static TimeSpan? GetHeatTime(IItemDatabase db, DateTime startTime)
        {
            LogRowDataPoint dataPoint = db.GetAllDP("Process.ProcessManager.HeatStartPlateDone").FirstOrDefault(p => (p.TimeStamp >= startTime && p.Value > 0.5));
            if (dataPoint == null)
                return null;

            return dataPoint.TimeStamp - startTime;
        }

        /// <summary>
        /// Get the time from the build end time until bottom temperature reached 100 degrees
        /// </summary>
        /// <param name="db"></param>
        /// <param name="endTime">Time when build is stopped</param>
        /// <returns></returns>
        public static TimeSpan? GetCoolingDownTime(IItemDatabase db, DateTime endTime)
        {
            LogRowDataPoint dataPoint = db.GetAllDP("OPC.Temperature.BottomTemperature").FirstOrDefault(p => (p.TimeStamp >= endTime && p.Value <= 110d));
            if (dataPoint == null)
                return null;

            return dataPoint.TimeStamp - endTime;
        }

        /// <summary>
        /// Get the time from the build start time until bottom temperature reached 100 degrees
        /// </summary>
        /// <param name="db"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime">Time when build is stopped</param>
        /// <returns></returns>
        public static TimeSpan? GetTotalBuildTime(IItemDatabase db, DateTime startTime, DateTime endTime)
        {
            LogRowDataPoint dataPoint = db.GetFirstItemDP("OPC.Temperature.BottomTemperature", p => (p.TimeStamp >= endTime && p.Value <= 110d));
            if (dataPoint == null)
                dataPoint = db.GetLastItemDP("OPC.Temperature.BottomTemperature");

            return dataPoint.TimeStamp - startTime;
        }

        /// <summary>
        /// Get the time from the first layer until Internal Process State not running
        /// </summary>
        /// <param name="db"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static TimeSpan? GetMeltTime(IItemDatabase db, DateTime startTime, DateTime endTime)
        {
            LogRowDataPoint processStopped = db.GetLastItemDP("Process.ProcessManager.InternalProcessStop",
                                                              p => p.TimeStamp > startTime && p.Value >= 0.5);

            if (processStopped == null)
                return null;

            LogRowDataPoint firstLayer = db.GetFirstItemDP("Beam.LayerThickness", p => p.TimeStamp >= startTime && p.TimeStamp <= endTime && p.Value > 0);
            if (firstLayer == null)
                return null;
            //throw new ApplicationException("Did not find the time of the first layer in the log file.");

            return processStopped.TimeStamp - firstLayer.TimeStamp;
        }
    }
}
