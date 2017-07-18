using System;

public class RenderWorker : WorkerThreadBase<RenderWorker>
{
    protected override void _PrepareWorkerData()
    {
        FogOfWar.PrepareData();
    }

    protected override void _Run()
    {
        FogOfWar.Run();
    }

    protected override void BeforeStart()
    {
        base.BeforeStart();
    }

    public void BeginLevel()
    {
        base.GetLock();
        try
        {
            FogOfWar.BeginLevel();
        }
        catch (Exception)
        {
        }
        finally
        {
            base.ReleaseLock();
        }
    }

    public void EndLevel()
    {
        base.GetLock();
        try
        {
            FogOfWar.EndLevel();
        }
        catch (Exception)
        {
        }
        finally
        {
            base.ReleaseLock();
        }
    }

    public void PreBeginLevel()
    {
        base.GetLock();
        try
        {
            FogOfWar.PreBeginLevel();
        }
        catch (Exception)
        {
        }
        finally
        {
            base.ReleaseLock();
        }
    }
}

