# learndocsmcpconsole

This shows minimal clients for the Learn Docs MCP Server:

* AIInerence - call the MCP Server with language model 
* SkPlugin - call the MCP Server with language model and Semantic Kernel agent

The samples are configured to use GitHub Models. If you run the samples in GitHub Codespaces, they should run automatically with GitHub Models. 

Curling is a list of curl commands using REST with AI Foundry.

TODO: Add AI Foundry sample.

Note about token limits:

- If you use models with token limits (for example `gpt-4o-mini` with an 8000 token limit), avoid sending large objects such as full tool metadata as part of the chat request. The sample `AIInference.cs` was updated to send a short comma-separated list of tool names instead of the full `tools` payload to prevent "Request body too large" errors.