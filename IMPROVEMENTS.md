# Improvements — data_structure_tree

**Goal:** Turn the throwaway console BST into an interactive, deployed "algorithm visualizer" web app where users build a tree and watch traversals/operations animate step-by-step, backed by a clean .NET API.

## TL;DR — Path to production
- [x] Fix insert/ancestor bugs → clean, generic `BinaryTree<T>` library + tests
- [x] ASP.NET Core API + Swagger
- [x] Web visualizer with animated traversals (plain HTML+JS+SVG in `wwwroot` — chosen over a React SPA for zero build tooling)
- [x] Docker + GitHub Actions CI
- [ ] Deployed public live demo

## Current state
- ~300-line net6.0 console app: `Node`, `BinTree`, and a hardcoded `Program.Main` demo.
- BST keyed on string *length*; supports insert, in/pre/post-order, contains, longest, ancestor.
- Buggy/unfinished: `insertItem` conflates `BinTree`/`Node` and uses `ref` wrongly; `Ancestor` returns "Success" not the node; dead commented int-search code; `Console.WriteLine` debug noise throughout.
- No tests, no CI, no packaging, project/namespace named `TaskA` (coursework origin).

## Key improvements
- Core: extract a clean, generic `BinaryTree<T>` library (.NET class lib), fix the insert/ancestor bugs, remove console side-effects, add a proper BST comparator.
- API: ASP.NET Core minimal API (`/insert`, `/traverse`, `/lca`, `/tree`) returning JSON node/edge graph; OpenAPI/Swagger.
- Frontend: React + TypeScript SPA with animated tree rendering (D3 / React Flow) that steps through each traversal and highlights the visited node.
- Quality: xUnit + FluentAssertions unit tests, property-based tests (FsCheck), coverage gate.
- DevEx: rename `TaskA` to a real solution name, add README with live demo GIF and architecture diagram.

## Latest tech to showcase
- .NET 9 + ASP.NET Core minimal APIs, C# 13.
- React 19 + TypeScript + Vite, React Flow / D3 for graph animation.
- Docker multi-stage build; GitHub Actions CI (build, test, lint) + deploy.
- Azure Container Apps or Fly.io for the live demo; OpenAPI-generated client.
- OpenTelemetry traces/metrics for the API.

## Roadmap
1. Refactor to a tested `BinaryTree<T>` lib on .NET 9; fix bugs, kill console noise.
2. Wrap in an ASP.NET Core API with Swagger + Dockerfile + CI.
3. Ship the React visualizer and deploy end-to-end with a live demo link.
