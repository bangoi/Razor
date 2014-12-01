// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Razor.Runtime.TagHelpers
{
    /// <summary>
    /// Class used to store information about a <see cref="ITagHelper"/>'s execution lifetime.
    /// </summary>
    public class TagHelperExecutionContext
    {
        private readonly List<ITagHelper> _tagHelpers;
        private string _childContent;

        /// <summary>
        /// Instantiates a new <see cref="TagHelperExecutionContext"/>.
        /// </summary>
        /// <param name="tagName">The HTML tag name in the Razor source.</param>
        /// <param name="uniqueId">An identifier unique to the HTML element this context is for.</param>
        /// <param name="executeChildContentAsync">A delegate used to execute the child content asynchronously.</param>
        /// <param name="startWritingScope">A delegate used to start a writing scope in a Razor page.</param>
        /// <param name="endWritingScope">A delegate used to end a writing scope in a Razor page.</param>
        public TagHelperExecutionContext([NotNull] string tagName,
				         [NotNull] string uniqueId,
                                         [NotNull] Func<Task> executeChildContentAsync,
                                         [NotNull] Action startWritingScope,
                                         [NotNull] Func<TextWriter> endWritingScope)
        {
            ExecuteChildContentAsync = executeChildContentAsync;
            GetChildContentAsync = async () =>
            {
                if (_childContent == null)
                {
                    startWritingScope();
                    await executeChildContentAsync();
                    _childContent = endWritingScope().ToString();
                }

                return _childContent;
            };
            AllAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            HTMLAttributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _tagHelpers = new List<ITagHelper>();
            TagName = tagName;
            UniqueId = uniqueId;
        }

        /// <summary>
        /// Internal for testing purposes only.
        /// </summary>
        internal TagHelperExecutionContext([NotNull] string tagName)
            : this(tagName, string.Empty)
        {

        }

        /// <summary>
        /// A delegate used to execute the child content asynchronously.
        /// </summary>
        public Func<Task> ExecuteChildContentAsync { get; }

        /// <summary>
        /// A delegate used to execute and retrieve the rendered child content asynchronously.
        /// </summary>
        /// <remarks>
        /// Child content is only executed once. Successive calls to this delegate return a cached result.
        /// </remarks>
        public Func<Task<string>> GetChildContentAsync { get; }

        /// <summary>
        /// Indicates if <see cref="GetChildContentAsync"/> has been called.
        /// </summary>
        public bool ChildContentRetrieved
        {
            get
            {
                return _childContent != null;
            }
        }

        /// <summary>
        /// HTML attributes.
        /// </summary>
        public IDictionary<string, string> HTMLAttributes { get; }

        /// <summary>
        /// <see cref="ITagHelper"/> bound attributes and HTML attributes.
        /// </summary>
        public IDictionary<string, object> AllAttributes { get; }

        /// <summary>
        /// An identifier unique to the HTML element this context is for.
        /// </summary>
        public string UniqueId { get; }

        /// <summary>
        /// <see cref="ITagHelper"/>s that should be run.
        /// </summary>
        public IEnumerable<ITagHelper> TagHelpers
        {
            get
            {
                return _tagHelpers;
            }
        }

        /// <summary>
        /// The HTML tag name in the Razor source.
        /// </summary>
        public string TagName { get; }

        /// <summary>
        /// The <see cref="ITagHelper"/>s' output.
        /// </summary>
        public TagHelperOutput Output { get; set; }

        /// <summary>
        /// Tracks the given <paramref name="tagHelper"/>.
        /// </summary>
        /// <param name="tagHelper">The tag helper to track.</param>
        public void Add([NotNull] ITagHelper tagHelper)
        {
            _tagHelpers.Add(tagHelper);
        }

        /// <summary>
        /// Tracks the HTML attribute in <see cref="AllAttributes"/> and <see cref="HTMLAttributes"/>.
        /// </summary>
        /// <param name="name">The HTML attribute name.</param>
        /// <param name="value">The HTML attribute value.</param>
        public void AddHtmlAttribute([NotNull] string name, string value)
        {
            HTMLAttributes.Add(name, value);
            AllAttributes.Add(name, value);
        }

        /// <summary>
        /// Tracks the <see cref="ITagHelper"/> bound attribute in <see cref="AllAttributes"/>.
        /// </summary>
        /// <param name="name">The bound attribute name.</param>
        /// <param name="value">The attribute value.</param>
        public void AddTagHelperAttribute([NotNull] string name, object value)
        {
            AllAttributes.Add(name, value);
        }
    }
}