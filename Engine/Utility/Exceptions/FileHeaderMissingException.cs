using System;

namespace Engine.Utility.Exceptions;

public class FileHeaderMissingException(string header) : Exception($"File is missing expected header: {header}") { }
