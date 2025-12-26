using Prong.Shared;
using Prong.Src;
using Xunit;

namespace Prong.Tests;

public class BoundedQueueTest
{
    [Fact]
    public void BoundedQueueIsStatic()
    {
        var sut = new BoundedQueue<int>(3);

        sut.Enqueue(1);
        sut.Enqueue(2);

        Assert.True(sut.Count() == 2);
        Assert.True(sut.Contains(1));
        Assert.True(sut.Contains(2));
    }
    
}
