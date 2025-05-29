using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.OperationResult
{
    public class OperationResultUnitTests
    {
        private readonly ResultError _error1 = ResultError.CustomForUnitTestsOnly(1, "Error 1");
        private readonly ResultError _error2 = ResultError.CustomForUnitTestsOnly(2, "Error 2");
        
        [Fact]
        public void Result_Success_HasNoErrors()
        {
            var result = Result.Success();

            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Result_Failure_WithSingleError()
        {
            var result = Result.Failure(_error1);

            Assert.True(result.IsFailure);
            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
            Assert.Equal(_error1.Code, result.Errors[0].Code);
        }

        [Fact]
        public void Result_Failure_WithMultipleErrors()
        {
            var errors = new[]
            {
                _error1,
                _error2
            };
            var result = Result.Failure(errors);

            Assert.True(result.IsFailure);
            Assert.Equal(2, result.Errors.Count);
        }

        [Fact]
        public void Result_Combine_MergesErrors()
        {
            var result1 = Result.Failure(_error1);
            var result2 = Result.Combine(result1, _error2);

            Assert.True(result2.IsFailure);
            Assert.Equal(2, result2.Errors.Count);
        }

        [Fact]
        public void ResultT_Success_HasPayload()
        {
            var result = Result<string>.Success("Hello");

            Assert.True(result.IsSuccess);
            Assert.Equal("Hello", result.Payload);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void ResultT_Failure_ThrowsOnEmptyErrors()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                Result<string>.Failure([]);
            });
            Assert.Equal("Failure must contain at least one error.", ex.Message);
        }

        [Fact]
        public void ResultT_Failure_WithOneError()
        {
            var result = Result<string>.Failure(_error1);

            Assert.True(result.IsFailure);
            Assert.Null(result.Payload);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void ResultT_Failure_WithMultipleErrors()
        {
            var result = Result<string>.Failure(
                _error1,
                _error2
            );

            Assert.True(result.IsFailure);
            Assert.Null(result.Payload);
            Assert.Equal(2, result.Errors.Count);
        }
    }
}
