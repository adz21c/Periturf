const { description } = require('../../package')

module.exports = {
  base: '/Periturf/',
  /**
   * Ref：https://v1.vuepress.vuejs.org/config/#title
   */
  title: 'Periturf',
  /**
   * Ref：https://v1.vuepress.vuejs.org/config/#description
   */
  description: 'Library to manage the stubbing and mocking of environment components.',

  /**
   * Extra tags to be injected to the page HTML `<head>`
   *
   * ref：https://v1.vuepress.vuejs.org/config/#head
   */
  head: [
    ['meta', { name: 'theme-color', content: '#3eaf7c' }],
    ['meta', { name: 'apple-mobile-web-app-capable', content: 'yes' }],
    ['meta', { name: 'apple-mobile-web-app-status-bar-style', content: 'black' }]
  ],

  /**
   * Theme configuration, here is the default theme configuration for VuePress.
   *
   * ref：https://v1.vuepress.vuejs.org/theme/default-theme-config.html
   */
  themeConfig: {
    repo: '',
    editLinks: false,
    docsDir: '',
    editLinkText: '',
    lastUpdated: false,
    nav: [
      {
        text: 'Using',
        link: '/using/',
      },
      {
        text: 'Extending',
        link: '/extending/'
      }
    ]
  },

  markdown: {
    lineNumbers: true
  },  

  evergreen: true,

  /**
   * Apply plugins，ref：https://v1.vuepress.vuejs.org/zh/plugin/
   */
  plugins: [
    '@vuepress/plugin-back-to-top',
    '@vuepress/plugin-medium-zoom',
    'mermaidjs'
  ]
}
