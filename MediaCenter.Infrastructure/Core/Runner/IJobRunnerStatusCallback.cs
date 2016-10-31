using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCenter.Infrastructure.Core.Runner
{
    public interface IJobRunnerStatusCallback
    {
        void OnJobStart(JobRunner jbr);
        void OnJobEnd(JobRunner jbr);
        void OnJobInit(JobRunner jbr);
        void OnJobUninit(JobRunner jbr);
    }
}
