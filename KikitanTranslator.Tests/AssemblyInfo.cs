using Xunit;

// The formatters read AppConfig.ConfigObject, which is process-wide mutable state, so
// test classes must not run concurrently or they overwrite each other's configuration.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
