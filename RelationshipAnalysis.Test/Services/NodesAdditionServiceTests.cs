
    [Fact]
    public async Task AddNodes_ShouldReturnBadRequestAndRollBack_WhenDbFailsToAddData()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.ValidFileMessage),
            StatusCode = StatusCodeType.Success
        };
        var csvContent = @"""AccountID"",""CardID"",""IBAN""
""6534454617"",""6104335000000190"",""IR120778801496000000198""
""6534454617"",""6104335000000190"",""IR120778801496000000198""
""4000000028"",""6037699000000020"",""IR033880987114000000028""
";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "AccountID").Returns(expected);
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        processorMock.ProcessCsvAsync(fileToBeSend).Returns(new List<dynamic>(){ new Dictionary<string, object>() });
        var additionServiceMock = new Mock<ISingleNodeAdditionService>();
        
        additionServiceMock
            .Setup(service => service.AddSingleNode(
                It.IsAny<ApplicationDbContext>(),
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<string>(),
                It.IsAny<int>()
            ))
            .ThrowsAsync(new Exception("Custom exception message"));
        _sut = new NodesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock.Object);

        // Act
        var result = await _sut.AddNodes(new UploadNodeDto()
        {
            File = fileToBeSend,
            NodeCategoryName = "Account",
            UniqueKeyHeaderName = "AccountID"
        });
        // Assert
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.Equal(0, context.Nodes.Count());
        Assert.Equal("Custom exception message", result.Data.Message);
    }
    
    
    private IFormFile CreateFileMock(string csvContent)
    {
        var csvFileName = "test.csv";
        var fileMock = Substitute.For<IFormFile>();
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(csvContent);
        writer.Flush();
        stream.Position = 0;

        fileMock.OpenReadStream().Returns(stream);
        fileMock.FileName.Returns(csvFileName);
        fileMock.Length.Returns(stream.Length);
        return fileMock;
    }
}