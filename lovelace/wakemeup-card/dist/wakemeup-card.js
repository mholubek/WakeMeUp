/**
 * @license
 * Copyright 2019 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const z = globalThis, Q = z.ShadowRoot && (z.ShadyCSS === void 0 || z.ShadyCSS.nativeShadow) && "adoptedStyleSheets" in Document.prototype && "replace" in CSSStyleSheet.prototype, G = Symbol(), ee = /* @__PURE__ */ new WeakMap();
let ge = class {
  constructor(e, t, s) {
    if (this._$cssResult$ = !0, s !== G) throw Error("CSSResult is not constructable. Use `unsafeCSS` or `css` instead.");
    this.cssText = e, this.t = t;
  }
  get styleSheet() {
    let e = this.o;
    const t = this.t;
    if (Q && e === void 0) {
      const s = t !== void 0 && t.length === 1;
      s && (e = ee.get(t)), e === void 0 && ((this.o = e = new CSSStyleSheet()).replaceSync(this.cssText), s && ee.set(t, e));
    }
    return e;
  }
  toString() {
    return this.cssText;
  }
};
const we = (r) => new ge(typeof r == "string" ? r : r + "", void 0, G), _e = (r, ...e) => {
  const t = r.length === 1 ? r[0] : e.reduce((s, i, a) => s + ((o) => {
    if (o._$cssResult$ === !0) return o.cssText;
    if (typeof o == "number") return o;
    throw Error("Value passed to 'css' function must be a 'css' function result: " + o + ". Use 'unsafeCSS' to pass non-literal values, but take care to ensure page security.");
  })(i) + r[a + 1], r[0]);
  return new ge(t, r, G);
}, Ee = (r, e) => {
  if (Q) r.adoptedStyleSheets = e.map((t) => t instanceof CSSStyleSheet ? t : t.styleSheet);
  else for (const t of e) {
    const s = document.createElement("style"), i = z.litNonce;
    i !== void 0 && s.setAttribute("nonce", i), s.textContent = t.cssText, r.appendChild(s);
  }
}, te = Q ? (r) => r : (r) => r instanceof CSSStyleSheet ? ((e) => {
  let t = "";
  for (const s of e.cssRules) t += s.cssText;
  return we(t);
})(r) : r;
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const { is: ke, defineProperty: Se, getOwnPropertyDescriptor: Ce, getOwnPropertyNames: Me, getOwnPropertySymbols: Pe, getPrototypeOf: Ue } = Object, y = globalThis, se = y.trustedTypes, Oe = se ? se.emptyScript : "", W = y.reactiveElementPolyfillSupport, U = (r, e) => r, I = { toAttribute(r, e) {
  switch (e) {
    case Boolean:
      r = r ? Oe : null;
      break;
    case Object:
    case Array:
      r = r == null ? r : JSON.stringify(r);
  }
  return r;
}, fromAttribute(r, e) {
  let t = r;
  switch (e) {
    case Boolean:
      t = r !== null;
      break;
    case Number:
      t = r === null ? null : Number(r);
      break;
    case Object:
    case Array:
      try {
        t = JSON.parse(r);
      } catch {
        t = null;
      }
  }
  return t;
} }, K = (r, e) => !ke(r, e), re = { attribute: !0, type: String, converter: I, reflect: !1, useDefault: !1, hasChanged: K };
Symbol.metadata ?? (Symbol.metadata = Symbol("metadata")), y.litPropertyMetadata ?? (y.litPropertyMetadata = /* @__PURE__ */ new WeakMap());
let E = class extends HTMLElement {
  static addInitializer(e) {
    this._$Ei(), (this.l ?? (this.l = [])).push(e);
  }
  static get observedAttributes() {
    return this.finalize(), this._$Eh && [...this._$Eh.keys()];
  }
  static createProperty(e, t = re) {
    if (t.state && (t.attribute = !1), this._$Ei(), this.prototype.hasOwnProperty(e) && ((t = Object.create(t)).wrapped = !0), this.elementProperties.set(e, t), !t.noAccessor) {
      const s = Symbol(), i = this.getPropertyDescriptor(e, s, t);
      i !== void 0 && Se(this.prototype, e, i);
    }
  }
  static getPropertyDescriptor(e, t, s) {
    const { get: i, set: a } = Ce(this.prototype, e) ?? { get() {
      return this[t];
    }, set(o) {
      this[t] = o;
    } };
    return { get: i, set(o) {
      const n = i == null ? void 0 : i.call(this);
      a == null || a.call(this, o), this.requestUpdate(e, n, s);
    }, configurable: !0, enumerable: !0 };
  }
  static getPropertyOptions(e) {
    return this.elementProperties.get(e) ?? re;
  }
  static _$Ei() {
    if (this.hasOwnProperty(U("elementProperties"))) return;
    const e = Ue(this);
    e.finalize(), e.l !== void 0 && (this.l = [...e.l]), this.elementProperties = new Map(e.elementProperties);
  }
  static finalize() {
    if (this.hasOwnProperty(U("finalized"))) return;
    if (this.finalized = !0, this._$Ei(), this.hasOwnProperty(U("properties"))) {
      const t = this.properties, s = [...Me(t), ...Pe(t)];
      for (const i of s) this.createProperty(i, t[i]);
    }
    const e = this[Symbol.metadata];
    if (e !== null) {
      const t = litPropertyMetadata.get(e);
      if (t !== void 0) for (const [s, i] of t) this.elementProperties.set(s, i);
    }
    this._$Eh = /* @__PURE__ */ new Map();
    for (const [t, s] of this.elementProperties) {
      const i = this._$Eu(t, s);
      i !== void 0 && this._$Eh.set(i, t);
    }
    this.elementStyles = this.finalizeStyles(this.styles);
  }
  static finalizeStyles(e) {
    const t = [];
    if (Array.isArray(e)) {
      const s = new Set(e.flat(1 / 0).reverse());
      for (const i of s) t.unshift(te(i));
    } else e !== void 0 && t.push(te(e));
    return t;
  }
  static _$Eu(e, t) {
    const s = t.attribute;
    return s === !1 ? void 0 : typeof s == "string" ? s : typeof e == "string" ? e.toLowerCase() : void 0;
  }
  constructor() {
    super(), this._$Ep = void 0, this.isUpdatePending = !1, this.hasUpdated = !1, this._$Em = null, this._$Ev();
  }
  _$Ev() {
    var e;
    this._$ES = new Promise((t) => this.enableUpdating = t), this._$AL = /* @__PURE__ */ new Map(), this._$E_(), this.requestUpdate(), (e = this.constructor.l) == null || e.forEach((t) => t(this));
  }
  addController(e) {
    var t;
    (this._$EO ?? (this._$EO = /* @__PURE__ */ new Set())).add(e), this.renderRoot !== void 0 && this.isConnected && ((t = e.hostConnected) == null || t.call(e));
  }
  removeController(e) {
    var t;
    (t = this._$EO) == null || t.delete(e);
  }
  _$E_() {
    const e = /* @__PURE__ */ new Map(), t = this.constructor.elementProperties;
    for (const s of t.keys()) this.hasOwnProperty(s) && (e.set(s, this[s]), delete this[s]);
    e.size > 0 && (this._$Ep = e);
  }
  createRenderRoot() {
    const e = this.shadowRoot ?? this.attachShadow(this.constructor.shadowRootOptions);
    return Ee(e, this.constructor.elementStyles), e;
  }
  connectedCallback() {
    var e;
    this.renderRoot ?? (this.renderRoot = this.createRenderRoot()), this.enableUpdating(!0), (e = this._$EO) == null || e.forEach((t) => {
      var s;
      return (s = t.hostConnected) == null ? void 0 : s.call(t);
    });
  }
  enableUpdating(e) {
  }
  disconnectedCallback() {
    var e;
    (e = this._$EO) == null || e.forEach((t) => {
      var s;
      return (s = t.hostDisconnected) == null ? void 0 : s.call(t);
    });
  }
  attributeChangedCallback(e, t, s) {
    this._$AK(e, s);
  }
  _$ET(e, t) {
    var a;
    const s = this.constructor.elementProperties.get(e), i = this.constructor._$Eu(e, s);
    if (i !== void 0 && s.reflect === !0) {
      const o = (((a = s.converter) == null ? void 0 : a.toAttribute) !== void 0 ? s.converter : I).toAttribute(t, s.type);
      this._$Em = e, o == null ? this.removeAttribute(i) : this.setAttribute(i, o), this._$Em = null;
    }
  }
  _$AK(e, t) {
    var a, o;
    const s = this.constructor, i = s._$Eh.get(e);
    if (i !== void 0 && this._$Em !== i) {
      const n = s.getPropertyOptions(i), l = typeof n.converter == "function" ? { fromAttribute: n.converter } : ((a = n.converter) == null ? void 0 : a.fromAttribute) !== void 0 ? n.converter : I;
      this._$Em = i;
      const p = l.fromAttribute(t, n.type);
      this[i] = p ?? ((o = this._$Ej) == null ? void 0 : o.get(i)) ?? p, this._$Em = null;
    }
  }
  requestUpdate(e, t, s, i = !1, a) {
    var o;
    if (e !== void 0) {
      const n = this.constructor;
      if (i === !1 && (a = this[e]), s ?? (s = n.getPropertyOptions(e)), !((s.hasChanged ?? K)(a, t) || s.useDefault && s.reflect && a === ((o = this._$Ej) == null ? void 0 : o.get(e)) && !this.hasAttribute(n._$Eu(e, s)))) return;
      this.C(e, t, s);
    }
    this.isUpdatePending === !1 && (this._$ES = this._$EP());
  }
  C(e, t, { useDefault: s, reflect: i, wrapped: a }, o) {
    s && !(this._$Ej ?? (this._$Ej = /* @__PURE__ */ new Map())).has(e) && (this._$Ej.set(e, o ?? t ?? this[e]), a !== !0 || o !== void 0) || (this._$AL.has(e) || (this.hasUpdated || s || (t = void 0), this._$AL.set(e, t)), i === !0 && this._$Em !== e && (this._$Eq ?? (this._$Eq = /* @__PURE__ */ new Set())).add(e));
  }
  async _$EP() {
    this.isUpdatePending = !0;
    try {
      await this._$ES;
    } catch (t) {
      Promise.reject(t);
    }
    const e = this.scheduleUpdate();
    return e != null && await e, !this.isUpdatePending;
  }
  scheduleUpdate() {
    return this.performUpdate();
  }
  performUpdate() {
    var s;
    if (!this.isUpdatePending) return;
    if (!this.hasUpdated) {
      if (this.renderRoot ?? (this.renderRoot = this.createRenderRoot()), this._$Ep) {
        for (const [a, o] of this._$Ep) this[a] = o;
        this._$Ep = void 0;
      }
      const i = this.constructor.elementProperties;
      if (i.size > 0) for (const [a, o] of i) {
        const { wrapped: n } = o, l = this[a];
        n !== !0 || this._$AL.has(a) || l === void 0 || this.C(a, void 0, o, l);
      }
    }
    let e = !1;
    const t = this._$AL;
    try {
      e = this.shouldUpdate(t), e ? (this.willUpdate(t), (s = this._$EO) == null || s.forEach((i) => {
        var a;
        return (a = i.hostUpdate) == null ? void 0 : a.call(i);
      }), this.update(t)) : this._$EM();
    } catch (i) {
      throw e = !1, this._$EM(), i;
    }
    e && this._$AE(t);
  }
  willUpdate(e) {
  }
  _$AE(e) {
    var t;
    (t = this._$EO) == null || t.forEach((s) => {
      var i;
      return (i = s.hostUpdated) == null ? void 0 : i.call(s);
    }), this.hasUpdated || (this.hasUpdated = !0, this.firstUpdated(e)), this.updated(e);
  }
  _$EM() {
    this._$AL = /* @__PURE__ */ new Map(), this.isUpdatePending = !1;
  }
  get updateComplete() {
    return this.getUpdateComplete();
  }
  getUpdateComplete() {
    return this._$ES;
  }
  shouldUpdate(e) {
    return !0;
  }
  update(e) {
    this._$Eq && (this._$Eq = this._$Eq.forEach((t) => this._$ET(t, this[t]))), this._$EM();
  }
  updated(e) {
  }
  firstUpdated(e) {
  }
};
E.elementStyles = [], E.shadowRootOptions = { mode: "open" }, E[U("elementProperties")] = /* @__PURE__ */ new Map(), E[U("finalized")] = /* @__PURE__ */ new Map(), W == null || W({ ReactiveElement: E }), (y.reactiveElementVersions ?? (y.reactiveElementVersions = [])).push("2.1.2");
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const O = globalThis, ie = (r) => r, L = O.trustedTypes, ae = L ? L.createPolicy("lit-html", { createHTML: (r) => r }) : void 0, fe = "$lit$", v = `lit$${Math.random().toFixed(9).slice(2)}$`, be = "?" + v, Te = `<${be}>`, w = document, T = () => w.createComment(""), D = (r) => r === null || typeof r != "object" && typeof r != "function", J = Array.isArray, De = (r) => J(r) || typeof (r == null ? void 0 : r[Symbol.iterator]) == "function", B = `[ 	
\f\r]`, M = /<(?:(!--|\/[^a-zA-Z])|(\/?[a-zA-Z][^>\s]*)|(\/?$))/g, oe = /-->/g, ne = />/g, A = RegExp(`>|${B}(?:([^\\s"'>=/]+)(${B}*=${B}*(?:[^ 	
\f\r"'\`<>=]|("|')|))|$)`, "g"), le = /'/g, de = /"/g, ve = /^(?:script|style|textarea|title)$/i, He = (r) => (e, ...t) => ({ _$litType$: r, strings: e, values: t }), h = He(1), S = Symbol.for("lit-noChange"), d = Symbol.for("lit-nothing"), ce = /* @__PURE__ */ new WeakMap(), $ = w.createTreeWalker(w, 129);
function ye(r, e) {
  if (!J(r) || !r.hasOwnProperty("raw")) throw Error("invalid template strings array");
  return ae !== void 0 ? ae.createHTML(e) : e;
}
const je = (r, e) => {
  const t = r.length - 1, s = [];
  let i, a = e === 2 ? "<svg>" : e === 3 ? "<math>" : "", o = M;
  for (let n = 0; n < t; n++) {
    const l = r[n];
    let p, m, c = -1, f = 0;
    for (; f < l.length && (o.lastIndex = f, m = o.exec(l), m !== null); ) f = o.lastIndex, o === M ? m[1] === "!--" ? o = oe : m[1] !== void 0 ? o = ne : m[2] !== void 0 ? (ve.test(m[2]) && (i = RegExp("</" + m[2], "g")), o = A) : m[3] !== void 0 && (o = A) : o === A ? m[0] === ">" ? (o = i ?? M, c = -1) : m[1] === void 0 ? c = -2 : (c = o.lastIndex - m[2].length, p = m[1], o = m[3] === void 0 ? A : m[3] === '"' ? de : le) : o === de || o === le ? o = A : o === oe || o === ne ? o = M : (o = A, i = void 0);
    const b = o === A && r[n + 1].startsWith("/>") ? " " : "";
    a += o === M ? l + Te : c >= 0 ? (s.push(p), l.slice(0, c) + fe + l.slice(c) + v + b) : l + v + (c === -2 ? n : b);
  }
  return [ye(r, a + (r[t] || "<?>") + (e === 2 ? "</svg>" : e === 3 ? "</math>" : "")), s];
};
class H {
  constructor({ strings: e, _$litType$: t }, s) {
    let i;
    this.parts = [];
    let a = 0, o = 0;
    const n = e.length - 1, l = this.parts, [p, m] = je(e, t);
    if (this.el = H.createElement(p, s), $.currentNode = this.el.content, t === 2 || t === 3) {
      const c = this.el.content.firstChild;
      c.replaceWith(...c.childNodes);
    }
    for (; (i = $.nextNode()) !== null && l.length < n; ) {
      if (i.nodeType === 1) {
        if (i.hasAttributes()) for (const c of i.getAttributeNames()) if (c.endsWith(fe)) {
          const f = m[o++], b = i.getAttribute(c).split(v), R = /([.?@])?(.*)/.exec(f);
          l.push({ type: 1, index: a, name: R[2], strings: b, ctor: R[1] === "." ? Re : R[1] === "?" ? ze : R[1] === "@" ? Ie : q }), i.removeAttribute(c);
        } else c.startsWith(v) && (l.push({ type: 6, index: a }), i.removeAttribute(c));
        if (ve.test(i.tagName)) {
          const c = i.textContent.split(v), f = c.length - 1;
          if (f > 0) {
            i.textContent = L ? L.emptyScript : "";
            for (let b = 0; b < f; b++) i.append(c[b], T()), $.nextNode(), l.push({ type: 2, index: ++a });
            i.append(c[f], T());
          }
        }
      } else if (i.nodeType === 8) if (i.data === be) l.push({ type: 2, index: a });
      else {
        let c = -1;
        for (; (c = i.data.indexOf(v, c + 1)) !== -1; ) l.push({ type: 7, index: a }), c += v.length - 1;
      }
      a++;
    }
  }
  static createElement(e, t) {
    const s = w.createElement("template");
    return s.innerHTML = e, s;
  }
}
function C(r, e, t = r, s) {
  var o, n;
  if (e === S) return e;
  let i = s !== void 0 ? (o = t._$Co) == null ? void 0 : o[s] : t._$Cl;
  const a = D(e) ? void 0 : e._$litDirective$;
  return (i == null ? void 0 : i.constructor) !== a && ((n = i == null ? void 0 : i._$AO) == null || n.call(i, !1), a === void 0 ? i = void 0 : (i = new a(r), i._$AT(r, t, s)), s !== void 0 ? (t._$Co ?? (t._$Co = []))[s] = i : t._$Cl = i), i !== void 0 && (e = C(r, i._$AS(r, e.values), i, s)), e;
}
class Ne {
  constructor(e, t) {
    this._$AV = [], this._$AN = void 0, this._$AD = e, this._$AM = t;
  }
  get parentNode() {
    return this._$AM.parentNode;
  }
  get _$AU() {
    return this._$AM._$AU;
  }
  u(e) {
    const { el: { content: t }, parts: s } = this._$AD, i = ((e == null ? void 0 : e.creationScope) ?? w).importNode(t, !0);
    $.currentNode = i;
    let a = $.nextNode(), o = 0, n = 0, l = s[0];
    for (; l !== void 0; ) {
      if (o === l.index) {
        let p;
        l.type === 2 ? p = new N(a, a.nextSibling, this, e) : l.type === 1 ? p = new l.ctor(a, l.name, l.strings, this, e) : l.type === 6 && (p = new Le(a, this, e)), this._$AV.push(p), l = s[++n];
      }
      o !== (l == null ? void 0 : l.index) && (a = $.nextNode(), o++);
    }
    return $.currentNode = w, i;
  }
  p(e) {
    let t = 0;
    for (const s of this._$AV) s !== void 0 && (s.strings !== void 0 ? (s._$AI(e, s, t), t += s.strings.length - 2) : s._$AI(e[t])), t++;
  }
}
class N {
  get _$AU() {
    var e;
    return ((e = this._$AM) == null ? void 0 : e._$AU) ?? this._$Cv;
  }
  constructor(e, t, s, i) {
    this.type = 2, this._$AH = d, this._$AN = void 0, this._$AA = e, this._$AB = t, this._$AM = s, this.options = i, this._$Cv = (i == null ? void 0 : i.isConnected) ?? !0;
  }
  get parentNode() {
    let e = this._$AA.parentNode;
    const t = this._$AM;
    return t !== void 0 && (e == null ? void 0 : e.nodeType) === 11 && (e = t.parentNode), e;
  }
  get startNode() {
    return this._$AA;
  }
  get endNode() {
    return this._$AB;
  }
  _$AI(e, t = this) {
    e = C(this, e, t), D(e) ? e === d || e == null || e === "" ? (this._$AH !== d && this._$AR(), this._$AH = d) : e !== this._$AH && e !== S && this._(e) : e._$litType$ !== void 0 ? this.$(e) : e.nodeType !== void 0 ? this.T(e) : De(e) ? this.k(e) : this._(e);
  }
  O(e) {
    return this._$AA.parentNode.insertBefore(e, this._$AB);
  }
  T(e) {
    this._$AH !== e && (this._$AR(), this._$AH = this.O(e));
  }
  _(e) {
    this._$AH !== d && D(this._$AH) ? this._$AA.nextSibling.data = e : this.T(w.createTextNode(e)), this._$AH = e;
  }
  $(e) {
    var a;
    const { values: t, _$litType$: s } = e, i = typeof s == "number" ? this._$AC(e) : (s.el === void 0 && (s.el = H.createElement(ye(s.h, s.h[0]), this.options)), s);
    if (((a = this._$AH) == null ? void 0 : a._$AD) === i) this._$AH.p(t);
    else {
      const o = new Ne(i, this), n = o.u(this.options);
      o.p(t), this.T(n), this._$AH = o;
    }
  }
  _$AC(e) {
    let t = ce.get(e.strings);
    return t === void 0 && ce.set(e.strings, t = new H(e)), t;
  }
  k(e) {
    J(this._$AH) || (this._$AH = [], this._$AR());
    const t = this._$AH;
    let s, i = 0;
    for (const a of e) i === t.length ? t.push(s = new N(this.O(T()), this.O(T()), this, this.options)) : s = t[i], s._$AI(a), i++;
    i < t.length && (this._$AR(s && s._$AB.nextSibling, i), t.length = i);
  }
  _$AR(e = this._$AA.nextSibling, t) {
    var s;
    for ((s = this._$AP) == null ? void 0 : s.call(this, !1, !0, t); e !== this._$AB; ) {
      const i = ie(e).nextSibling;
      ie(e).remove(), e = i;
    }
  }
  setConnected(e) {
    var t;
    this._$AM === void 0 && (this._$Cv = e, (t = this._$AP) == null || t.call(this, e));
  }
}
class q {
  get tagName() {
    return this.element.tagName;
  }
  get _$AU() {
    return this._$AM._$AU;
  }
  constructor(e, t, s, i, a) {
    this.type = 1, this._$AH = d, this._$AN = void 0, this.element = e, this.name = t, this._$AM = i, this.options = a, s.length > 2 || s[0] !== "" || s[1] !== "" ? (this._$AH = Array(s.length - 1).fill(new String()), this.strings = s) : this._$AH = d;
  }
  _$AI(e, t = this, s, i) {
    const a = this.strings;
    let o = !1;
    if (a === void 0) e = C(this, e, t, 0), o = !D(e) || e !== this._$AH && e !== S, o && (this._$AH = e);
    else {
      const n = e;
      let l, p;
      for (e = a[0], l = 0; l < a.length - 1; l++) p = C(this, n[s + l], t, l), p === S && (p = this._$AH[l]), o || (o = !D(p) || p !== this._$AH[l]), p === d ? e = d : e !== d && (e += (p ?? "") + a[l + 1]), this._$AH[l] = p;
    }
    o && !i && this.j(e);
  }
  j(e) {
    e === d ? this.element.removeAttribute(this.name) : this.element.setAttribute(this.name, e ?? "");
  }
}
class Re extends q {
  constructor() {
    super(...arguments), this.type = 3;
  }
  j(e) {
    this.element[this.name] = e === d ? void 0 : e;
  }
}
class ze extends q {
  constructor() {
    super(...arguments), this.type = 4;
  }
  j(e) {
    this.element.toggleAttribute(this.name, !!e && e !== d);
  }
}
class Ie extends q {
  constructor(e, t, s, i, a) {
    super(e, t, s, i, a), this.type = 5;
  }
  _$AI(e, t = this) {
    if ((e = C(this, e, t, 0) ?? d) === S) return;
    const s = this._$AH, i = e === d && s !== d || e.capture !== s.capture || e.once !== s.once || e.passive !== s.passive, a = e !== d && (s === d || i);
    i && this.element.removeEventListener(this.name, this, s), a && this.element.addEventListener(this.name, this, e), this._$AH = e;
  }
  handleEvent(e) {
    var t;
    typeof this._$AH == "function" ? this._$AH.call(((t = this.options) == null ? void 0 : t.host) ?? this.element, e) : this._$AH.handleEvent(e);
  }
}
class Le {
  constructor(e, t, s) {
    this.element = e, this.type = 6, this._$AN = void 0, this._$AM = t, this.options = s;
  }
  get _$AU() {
    return this._$AM._$AU;
  }
  _$AI(e) {
    C(this, e);
  }
}
const F = O.litHtmlPolyfillSupport;
F == null || F(H, N), (O.litHtmlVersions ?? (O.litHtmlVersions = [])).push("3.3.2");
const qe = (r, e, t) => {
  const s = (t == null ? void 0 : t.renderBefore) ?? e;
  let i = s._$litPart$;
  if (i === void 0) {
    const a = (t == null ? void 0 : t.renderBefore) ?? null;
    s._$litPart$ = i = new N(e.insertBefore(T(), a), a, void 0, t ?? {});
  }
  return i._$AI(r), i;
};
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const x = globalThis;
class k extends E {
  constructor() {
    super(...arguments), this.renderOptions = { host: this }, this._$Do = void 0;
  }
  createRenderRoot() {
    var t;
    const e = super.createRenderRoot();
    return (t = this.renderOptions).renderBefore ?? (t.renderBefore = e.firstChild), e;
  }
  update(e) {
    const t = this.render();
    this.hasUpdated || (this.renderOptions.isConnected = this.isConnected), super.update(e), this._$Do = qe(t, this.renderRoot, this.renderOptions);
  }
  connectedCallback() {
    var e;
    super.connectedCallback(), (e = this._$Do) == null || e.setConnected(!0);
  }
  disconnectedCallback() {
    var e;
    super.disconnectedCallback(), (e = this._$Do) == null || e.setConnected(!1);
  }
  render() {
    return S;
  }
}
var me;
k._$litElement$ = !0, k.finalized = !0, (me = x.litElementHydrateSupport) == null || me.call(x, { LitElement: k });
const V = x.litElementPolyfillSupport;
V == null || V({ LitElement: k });
(x.litElementVersions ?? (x.litElementVersions = [])).push("4.2.2");
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const Ae = (r) => (e, t) => {
  t !== void 0 ? t.addInitializer(() => {
    customElements.define(r, e);
  }) : customElements.define(r, e);
};
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const We = { attribute: !0, type: String, converter: I, reflect: !1, hasChanged: K }, Be = (r = We, e, t) => {
  const { kind: s, metadata: i } = t;
  let a = globalThis.litPropertyMetadata.get(i);
  if (a === void 0 && globalThis.litPropertyMetadata.set(i, a = /* @__PURE__ */ new Map()), s === "setter" && ((r = Object.create(r)).wrapped = !0), a.set(t.name, r), s === "accessor") {
    const { name: o } = t;
    return { set(n) {
      const l = e.get.call(this);
      e.set.call(this, n), this.requestUpdate(o, l, r, !0, n);
    }, init(n) {
      return n !== void 0 && this.C(o, void 0, r, n), n;
    } };
  }
  if (s === "setter") {
    const { name: o } = t;
    return function(n) {
      const l = this[o];
      e.call(this, n), this.requestUpdate(o, l, r, !0, n);
    };
  }
  throw Error("Unsupported decorator location: " + s);
};
function Y(r) {
  return (e, t) => typeof t == "object" ? Be(r, e, t) : ((s, i, a) => {
    const o = i.hasOwnProperty(a);
    return i.constructor.createProperty(a, s), o ? Object.getOwnPropertyDescriptor(i, a) : void 0;
  })(r, e, t);
}
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
function _(r) {
  return Y({ ...r, state: !0, attribute: !1 });
}
const Fe = "wakemeup", Ve = (r, e) => `${r}${e.startsWith("/") ? e : `/${e}`}`, Ze = (r) => {
  if (!r || typeof r != "object")
    return;
  const e = Object.entries(r).map(([t, s]) => {
    const i = t && `${t.charAt(0).toLowerCase()}${t.slice(1)}`;
    return Array.isArray(s) ? [i, s.filter((a) => typeof a == "string")] : typeof s == "string" ? [i, [s]] : [i, []];
  }).filter(([, t]) => t.length > 0);
  if (e.length !== 0)
    return Object.fromEntries(e);
}, Qe = (r) => {
  if (!r || typeof r != "object")
    return { message: "Unexpected API error." };
  const e = r, t = (e.body && typeof e.body == "object" ? e.body : void 0) ?? (e.error && typeof e.error == "object" ? e.error : void 0);
  if (t) {
    const s = t, i = Array.isArray(s.details) ? Object.fromEntries(
      s.details.filter((n) => typeof n.field == "string" && typeof n.message == "string").map((n) => [n.field, [n.message]])
    ) : void 0, a = Ze(s.errors) ?? (i && Object.keys(i).length > 0 ? i : void 0), o = typeof s.error == "string" && s.error || typeof s.detail == "string" && s.detail || typeof s.title == "string" && s.title || typeof e.message == "string" && e.message || "Unexpected API error.";
    return {
      status: typeof e.status_code == "number" ? e.status_code : void 0,
      message: o,
      validation: a
    };
  }
  return {
    status: typeof e.status_code == "number" ? e.status_code : void 0,
    message: typeof e.message == "string" && e.message || typeof e.error == "string" && e.error || "Unexpected API error."
  };
};
class Ge {
  constructor(e) {
    this.hass = e;
  }
  async getMeta() {
    return this.request("GET", "/meta");
  }
  async getAlarms() {
    return this.request("GET", "/alarms");
  }
  async createAlarm(e) {
    return this.request("POST", "/alarms", e);
  }
  async updateAlarm(e, t) {
    return this.request("PUT", `/alarms/${encodeURIComponent(e)}`, t);
  }
  async setAllAlarmsEnabled(e) {
    return this.request("PATCH", "/alarms/enabled", {
      isEnabled: e
    });
  }
  async toggleAlarm(e, t) {
    return this.request("PATCH", `/alarms/${encodeURIComponent(e)}/enabled`, {
      isEnabled: t
    });
  }
  async deleteAlarm(e) {
    return this.request("DELETE", `/alarms/${encodeURIComponent(e)}`);
  }
  async request(e, t, s) {
    const i = Ve(Fe, t);
    try {
      return await this.hass.callApi(e, i, s);
    } catch (a) {
      throw Qe(a);
    }
  }
}
const $e = (r) => (r ?? []).map((e) => {
  if (typeof e == "string")
    return { value: e, label: e };
  const t = e.value ?? e.key ?? e.id ?? e.code, s = e.label ?? e.displayName ?? e.name ?? t;
  if (!(!t || !s))
    return { value: t, label: s };
}).filter((e) => !!e), Ke = (r) => $e(r), he = (r, e = "en") => {
  if (!r)
    return "--";
  const t = new Date(r);
  return Number.isNaN(t.getTime()) ? r : new Intl.DateTimeFormat(e, {
    dateStyle: "medium",
    timeStyle: "short"
  }).format(t);
}, P = (r) => r && typeof r == "object" && "message" in r ? r : r instanceof Error ? { message: r.message } : { message: "Unexpected API error." }, Je = {
  cardTitle: "WakeMeUp",
  addAlarm: "Add alarm",
  enableAllAlarms: "Enable all alarms",
  disableAllAlarms: "Disable all alarms",
  editAlarm: "Edit alarm",
  noAlarms: "No alarms yet",
  loading: "Loading alarms...",
  refresh: "Refresh",
  deleteAlarm: "Delete alarm",
  deleteConfirm: "Delete this alarm?",
  cancel: "Cancel",
  save: "Save",
  create: "Create",
  enabled: "Enabled",
  disabled: "Disabled",
  name: "Name",
  description: "Description",
  time: "Time",
  repeatMode: "Repeat mode",
  days: "Days",
  timezone: "Time zone",
  nextTrigger: "Next trigger",
  createdAt: "Created",
  lastTriggered: "Last triggered",
  lastResult: "Last result",
  emptyDescription: "No description",
  unnamedAlarm: "Untitled alarm",
  errorTitle: "Something went wrong",
  validationTitle: "Please review the highlighted fields.",
  retry: "Retry",
  close: "Close",
  requiredField: "This field is required.",
  formHelp: "Changes are saved directly through the WakeMeUp API.",
  unknownError: "Unexpected API error.",
  saveSuccess: "Alarm saved.",
  deleteSuccess: "Alarm deleted.",
  toggleSuccess: "Alarm updated.",
  bulkEnableSuccess: "All alarms enabled.",
  bulkDisableSuccess: "All alarms disabled."
}, xe = {
  de: {
    addAlarm: "Alarm hinzufügen",
    enableAllAlarms: "Alle Alarme aktivieren",
    disableAllAlarms: "Alle Alarme deaktivieren",
    editAlarm: "Alarm bearbeiten",
    noAlarms: "Noch keine Alarme",
    deleteAlarm: "Alarm löschen",
    refresh: "Aktualisieren",
    create: "Erstellen",
    save: "Speichern",
    cancel: "Abbrechen"
  },
  fr: {
    addAlarm: "Ajouter une alarme",
    enableAllAlarms: "Activer toutes les alarmes",
    disableAllAlarms: "Désactiver toutes les alarmes",
    editAlarm: "Modifier l'alarme",
    noAlarms: "Aucune alarme",
    deleteAlarm: "Supprimer l'alarme",
    refresh: "Actualiser",
    create: "Créer",
    save: "Enregistrer",
    cancel: "Annuler"
  },
  es: {
    addAlarm: "Agregar alarma",
    enableAllAlarms: "Activar todas las alarmas",
    disableAllAlarms: "Desactivar todas las alarmas",
    editAlarm: "Editar alarma",
    noAlarms: "Sin alarmas",
    deleteAlarm: "Eliminar alarma",
    refresh: "Actualizar",
    create: "Crear",
    save: "Guardar",
    cancel: "Cancelar"
  },
  pt: {
    addAlarm: "Adicionar alarme",
    enableAllAlarms: "Ativar todos os alarmes",
    disableAllAlarms: "Desativar todos os alarmes",
    editAlarm: "Editar alarme",
    noAlarms: "Sem alarmes",
    deleteAlarm: "Eliminar alarme",
    refresh: "Atualizar",
    create: "Criar",
    save: "Guardar",
    cancel: "Cancelar"
  },
  it: {
    addAlarm: "Aggiungi allarme",
    enableAllAlarms: "Attiva tutti gli allarmi",
    disableAllAlarms: "Disattiva tutti gli allarmi",
    editAlarm: "Modifica allarme",
    noAlarms: "Nessun allarme",
    deleteAlarm: "Elimina allarme",
    refresh: "Aggiorna",
    create: "Crea",
    save: "Salva",
    cancel: "Annulla"
  },
  sk: {
    addAlarm: "Pridať alarm",
    enableAllAlarms: "Zapnúť všetky alarmy",
    disableAllAlarms: "Vypnúť všetky alarmy",
    editAlarm: "Upraviť alarm",
    noAlarms: "Zatiaľ žiadne alarmy",
    deleteAlarm: "Zmazať alarm",
    refresh: "Obnoviť",
    create: "Vytvoriť",
    save: "Uložiť",
    cancel: "Zrušiť"
  },
  cs: {
    addAlarm: "Přidat alarm",
    enableAllAlarms: "Zapnout všechny alarmy",
    disableAllAlarms: "Vypnout všechny alarmy",
    editAlarm: "Upravit alarm",
    noAlarms: "Zatím žádné alarmy",
    deleteAlarm: "Smazat alarm",
    refresh: "Obnovit",
    create: "Vytvořit",
    save: "Uložit",
    cancel: "Zrušit"
  },
  pl: {
    addAlarm: "Dodaj alarm",
    enableAllAlarms: "Włącz wszystkie alarmy",
    disableAllAlarms: "Wyłącz wszystkie alarmy",
    editAlarm: "Edytuj alarm",
    noAlarms: "Brak alarmów",
    deleteAlarm: "Usuń alarm",
    refresh: "Odśwież",
    create: "Utwórz",
    save: "Zapisz",
    cancel: "Anuluj"
  },
  uk: {
    addAlarm: "Додати будильник",
    enableAllAlarms: "Увімкнути всі будильники",
    disableAllAlarms: "Вимкнути всі будильники",
    editAlarm: "Редагувати будильник",
    noAlarms: "Будильників ще немає",
    deleteAlarm: "Видалити будильник",
    refresh: "Оновити",
    create: "Створити",
    save: "Зберегти",
    cancel: "Скасувати"
  },
  el: {
    addAlarm: "Προσθήκη ξυπνητηριού",
    enableAllAlarms: "Ενεργοποίηση όλων των ξυπνητηριών",
    disableAllAlarms: "Απενεργοποίηση όλων των ξυπνητηριών",
    editAlarm: "Επεξεργασία ξυπνητηριού",
    noAlarms: "Δεν υπάρχουν ξυπνητήρια",
    deleteAlarm: "Διαγραφή ξυπνητηριού",
    refresh: "Ανανέωση",
    create: "Δημιουργία",
    save: "Αποθήκευση",
    cancel: "Ακύρωση"
  },
  eo: {
    addAlarm: "Aldoni alarmon",
    enableAllAlarms: "Ŝalti ĉiujn alarmojn",
    disableAllAlarms: "Malŝalti ĉiujn alarmojn",
    editAlarm: "Redakti alarmon",
    noAlarms: "Ankoraŭ neniuj alarmoj",
    deleteAlarm: "Forigi alarmon",
    refresh: "Refreŝigi",
    create: "Krei",
    save: "Konservi",
    cancel: "Nuligi"
  },
  tlh: {
    addAlarm: "weQ chenmoH",
    enableAllAlarms: "weQmey boQHa'moH",
    disableAllAlarms: "weQmey QotlhHa'moH",
    editAlarm: "weQ choH",
    noAlarms: "weQ tu'be'lu'",
    deleteAlarm: "weQ Qaw'",
    refresh: "choHqa'",
    create: "chenmoH",
    save: "pol",
    cancel: "qIl"
  }
}, Ye = (r) => {
  if (!r)
    return "en";
  const e = r.toLowerCase();
  if (e.startsWith("tlh"))
    return "tlh";
  const t = e.split(/[-_]/)[0];
  return xe[t] ? t : "en";
}, pe = (r) => {
  const e = Ye(r);
  return {
    ...Je,
    ...xe[e] ?? {}
  };
};
var Xe = Object.defineProperty, et = Object.getOwnPropertyDescriptor, g = (r, e, t, s) => {
  for (var i = s > 1 ? void 0 : s ? et(e, t) : e, a = r.length - 1, o; a >= 0; a--)
    (o = r[a]) && (i = (s ? o(e, t, i) : o(i)) || i);
  return s && i && Xe(e, t, i), i;
};
const Z = (r) => {
  var e;
  return {
    name: "",
    description: "",
    time: "07:00",
    isEnabled: !0,
    repeatMode: ((e = r[0]) == null ? void 0 : e.value) ?? "",
    days: []
  };
}, tt = (r) => ({
  id: r.id,
  name: r.name ?? "",
  description: r.description ?? "",
  time: r.time ?? "07:00",
  isEnabled: r.isEnabled ?? !0,
  repeatMode: r.repeatMode ?? "",
  days: [...r.days ?? []]
}), st = (r) => [...r].sort((e, t) => e.time.localeCompare(t.time) || e.name.localeCompare(t.name)), ue = (r) => /(week|day|custom|selected|repeat)/i.test(r);
let u = class extends k {
  constructor() {
    super(...arguments), this._alarms = [], this._translations = pe(), this._repeatModes = [], this._weekdays = [], this._loading = !0, this._busy = !1, this._dialogOpen = !1, this._form = Z([]), this._openCreateDialog = () => {
      this._statusMessage = void 0, this._error = void 0, this._validation = void 0, this._form = Z(this._repeatModes), this._dialogOpen = !0;
    }, this._closeDialog = () => {
      this._dialogOpen = !1, this._validation = void 0, this._error = void 0;
    }, this._handleInput = (r) => {
      var i;
      const e = r.currentTarget, { name: t } = e, s = e instanceof HTMLInputElement && e.type === "checkbox" ? e.checked : e.value;
      if (this._form = {
        ...this._form,
        [t]: s
      }, t === "repeatMode" && !ue(String(s)) && (this._form = {
        ...this._form,
        days: []
      }), (i = this._validation) != null && i[t]) {
        const a = { ...this._validation };
        delete a[t], this._validation = Object.keys(a).length ? a : void 0;
      }
    };
  }
  static async getConfigElement() {
    return document.createElement("wakemeup-card-editor");
  }
  static getStubConfig() {
    return {
      type: "custom:wakemeup-card",
      title: "WakeMeUp"
    };
  }
  setConfig(r) {
    if (!r.type)
      throw new Error("Invalid configuration");
    this._config = r, this._client = void 0;
  }
  getCardSize() {
    return Math.max(3, this._alarms.length + 1);
  }
  willUpdate(r) {
    (r.has("hass") || r.has("_config")) && this.hass && this._config && (this._client = new Ge(this.hass));
  }
  updated(r) {
    (r.has("hass") || r.has("_config")) && this._client && this._loadData();
  }
  render() {
    var t, s;
    if (!this._config)
      return h``;
    const r = this._config.title || this._translations.cardTitle, e = ((t = this._meta) == null ? void 0 : t.currentLanguage) ?? "en";
    return h`
      <ha-card>
        <div class="shell">
          <div class="header">
            <div>
              <div class="eyebrow">${this._translations.timezone}</div>
              <h1>${r}</h1>
              <div class="subtle">${((s = this._meta) == null ? void 0 : s.timeZoneDisplayName) ?? "--"}</div>
            </div>
            <div class="actions">
              <button class="soft" @click=${() => this._setAllAlarmsEnabled(!0)} ?disabled=${this._loading || this._busy}>
                ${this._translations.enableAllAlarms}
              </button>
              <button class="soft" @click=${() => this._setAllAlarmsEnabled(!1)} ?disabled=${this._loading || this._busy}>
                ${this._translations.disableAllAlarms}
              </button>
              <button class="soft" @click=${this._loadData} ?disabled=${this._loading || this._busy}>
                ${this._translations.refresh}
              </button>
              <button class="primary" @click=${this._openCreateDialog} ?disabled=${this._loading || this._busy}>
                ${this._translations.addAlarm}
              </button>
            </div>
          </div>

          ${this._statusMessage ? h`<div class="banner success">${this._statusMessage}</div>` : d}
          ${this._error ? h`<div class="banner error">${this._error}</div>` : d}

          ${this._loading ? h`<div class="state">${this._translations.loading}</div>` : this._alarms.length === 0 ? h`<div class="state">${this._translations.noAlarms}</div>` : h`
                  <div class="list">
                    ${this._alarms.map(
      (i) => {
        var a;
        return h`
                        <article class="alarm">
                          <div class="alarm-main">
                            <div class="pill-row">
                              <span class="time-pill">${i.time}</span>
                              <label class="toggle">
                                <input
                                  type="checkbox"
                                  .checked=${i.isEnabled}
                                  @change=${(o) => this._toggleAlarm(i, o)}
                                  ?disabled=${this._busy}
                                />
                                <span>${i.isEnabled ? this._translations.enabled : this._translations.disabled}</span>
                              </label>
                            </div>

                            <div class="alarm-title">${i.name || this._translations.unnamedAlarm}</div>
                            <div class="alarm-description">
                              ${i.description || this._translations.emptyDescription}
                            </div>

                            <div class="meta-grid">
                              <div>
                                <span>${this._translations.repeatMode}</span>
                                <strong>${i.repeatMode}</strong>
                              </div>
                              <div>
                                <span>${this._translations.days}</span>
                                <strong>${((a = i.days) == null ? void 0 : a.join(", ")) || "--"}</strong>
                              </div>
                              <div>
                                <span>${this._translations.nextTrigger}</span>
                                <strong>${he(i.nextTriggerLocal, e)}</strong>
                              </div>
                              <div>
                                <span>${this._translations.lastTriggered}</span>
                                <strong>${he(i.lastTriggeredUtc, e)}</strong>
                              </div>
                            </div>

                            ${i.lastResultMessage ? h`
                                  <div class="result">
                                    <span>${this._translations.lastResult}</span>
                                    <strong>${i.lastResultMessage}</strong>
                                  </div>
                                ` : d}
                          </div>

                          <div class="alarm-actions">
                            <button class="soft" @click=${() => this._openEditDialog(i)} ?disabled=${this._busy}>
                              ${this._translations.editAlarm}
                            </button>
                            <button class="danger" @click=${() => this._deleteAlarm(i)} ?disabled=${this._busy}>
                              ${this._translations.deleteAlarm}
                            </button>
                          </div>
                        </article>
                      `;
      }
    )}
                  </div>
                `}

          ${this._dialogOpen ? this._renderDialog() : d}
        </div>
      </ha-card>
    `;
  }
  _renderDialog() {
    const r = this._form.id ? this._translations.save : this._translations.create, e = ue(this._form.repeatMode) && this._weekdays.length > 0;
    return h`
      <div class="overlay" @click=${this._closeDialog}>
        <div class="dialog" @click=${(t) => t.stopPropagation()}>
          <div class="dialog-head">
            <div>
              <div class="eyebrow">${this._translations.formHelp}</div>
              <h2>${this._form.id ? this._translations.editAlarm : this._translations.addAlarm}</h2>
            </div>
            <button class="icon" @click=${this._closeDialog}>${this._translations.close}</button>
          </div>

          ${this._validation ? h`<div class="banner error">${this._translations.validationTitle}</div>` : d}

          <form @submit=${this._submitForm}>
            ${this._renderField("name", this._translations.name, h`
              <input
                name="name"
                .value=${this._form.name}
                @input=${this._handleInput}
                required
              />
            `)}

            ${this._renderField("description", this._translations.description, h`
              <textarea
                name="description"
                .value=${this._form.description}
                @input=${this._handleInput}
                rows="3"
              ></textarea>
            `)}

            <div class="form-grid">
              ${this._renderField("time", this._translations.time, h`
                <input
                  type="time"
                  name="time"
                  .value=${this._form.time}
                  @input=${this._handleInput}
                  required
                />
              `)}

              ${this._renderField("repeatMode", this._translations.repeatMode, h`
                <select name="repeatMode" .value=${this._form.repeatMode} @change=${this._handleInput}>
                  ${this._repeatModes.map(
      (t) => h`<option value=${t.value}>${t.label}</option>`
    )}
                </select>
              `)}
            </div>

            <label class="checkbox-row">
              <input
                type="checkbox"
                name="isEnabled"
                .checked=${this._form.isEnabled}
                @change=${this._handleInput}
              />
              <span>${this._translations.enabled}</span>
            </label>

            ${e ? h`
                  <div class="field">
                    <span class="label">${this._translations.days}</span>
                    <div class="chips">
                      ${this._weekdays.map(
      (t) => h`
                          <label class="chip ${this._form.days.includes(t.value) ? "selected" : ""}">
                            <input
                              type="checkbox"
                              .checked=${this._form.days.includes(t.value)}
                              @change=${(s) => this._toggleDay(t.value, s)}
                            />
                            <span>${t.label}</span>
                          </label>
                        `
    )}
                    </div>
                    ${this._renderErrors("days")}
                  </div>
                ` : d}

            <div class="dialog-actions">
              <button type="button" class="soft" @click=${this._closeDialog}>${this._translations.cancel}</button>
              <button type="submit" class="primary" ?disabled=${this._busy}>${r}</button>
            </div>
          </form>
        </div>
      </div>
    `;
  }
  _renderField(r, e, t) {
    return h`
      <label class="field">
        <span class="label">${e}</span>
        ${t}
        ${this._renderErrors(r)}
      </label>
    `;
  }
  _renderErrors(r) {
    var t;
    const e = (t = this._validation) == null ? void 0 : t[r];
    return e != null && e.length ? h`<div class="field-error">${e.join(" ")}</div>` : d;
  }
  async _loadData() {
    if (this._client) {
      this._loading = !0, this._error = void 0, this._statusMessage = void 0;
      try {
        const [r, e] = await Promise.all([this._client.getMeta(), this._client.getAlarms()]);
        this._meta = r, this._translations = pe(r.currentLanguage), this._repeatModes = $e(r.repeatModes), this._weekdays = Ke(r.weekdays), this._alarms = st(e), this._form.id || (this._form = Z(this._repeatModes));
      } catch (r) {
        this._error = P(r).message;
      } finally {
        this._loading = !1;
      }
    }
  }
  _openEditDialog(r) {
    this._statusMessage = void 0, this._error = void 0, this._validation = void 0, this._form = tt(r), this._dialogOpen = !0;
  }
  _toggleDay(r, e) {
    const s = e.currentTarget.checked ? [...this._form.days, r] : this._form.days.filter((i) => i !== r);
    this._form = {
      ...this._form,
      days: s
    };
  }
  async _submitForm(r) {
    if (r.preventDefault(), !this._client)
      return;
    this._busy = !0, this._error = void 0, this._validation = void 0;
    const e = {
      name: this._form.name.trim(),
      description: this._form.description.trim(),
      time: this._form.time,
      isEnabled: this._form.isEnabled,
      repeatMode: this._form.repeatMode,
      days: this._form.days
    };
    try {
      this._form.id ? await this._client.updateAlarm(this._form.id, e) : await this._client.createAlarm(e), this._dialogOpen = !1, this._statusMessage = this._translations.saveSuccess, await this._loadData();
    } catch (t) {
      const s = P(t);
      this._error = s.message, this._validation = s.validation;
    } finally {
      this._busy = !1;
    }
  }
  async _toggleAlarm(r, e) {
    if (!this._client)
      return;
    const t = e.currentTarget;
    this._busy = !0, this._error = void 0, this._statusMessage = void 0;
    try {
      await this._client.toggleAlarm(r.id, t.checked), this._alarms = this._alarms.map(
        (s) => s.id === r.id ? { ...s, isEnabled: t.checked } : s
      ), this._statusMessage = this._translations.toggleSuccess;
    } catch (s) {
      t.checked = !t.checked, this._error = P(s).message;
    } finally {
      this._busy = !1;
    }
  }
  async _setAllAlarmsEnabled(r) {
    if (this._client) {
      this._busy = !0, this._error = void 0, this._statusMessage = void 0;
      try {
        await this._client.setAllAlarmsEnabled(r), await this._loadData(), this._statusMessage = r ? this._translations.bulkEnableSuccess : this._translations.bulkDisableSuccess;
      } catch (e) {
        this._error = P(e).message;
      } finally {
        this._busy = !1;
      }
    }
  }
  async _deleteAlarm(r) {
    if (!(!this._client || !window.confirm(this._translations.deleteConfirm))) {
      this._busy = !0, this._error = void 0, this._statusMessage = void 0;
      try {
        await this._client.deleteAlarm(r.id), this._alarms = this._alarms.filter((e) => e.id !== r.id), this._statusMessage = this._translations.deleteSuccess;
      } catch (e) {
        this._error = P(e).message;
      } finally {
        this._busy = !1;
      }
    }
  }
};
u.styles = _e`
    :host {
      display: block;
    }

    ha-card {
      border-radius: 28px;
      overflow: hidden;
      background:
        radial-gradient(circle at top left, color-mix(in srgb, var(--primary-color) 14%, transparent), transparent 45%),
        linear-gradient(
          160deg,
          color-mix(in srgb, var(--card-background-color) 92%, var(--primary-background-color)),
          color-mix(in srgb, var(--ha-card-background, var(--card-background-color)) 98%, black 2%)
        );
      box-shadow:
        0 18px 40px rgba(0, 0, 0, 0.12),
        inset 0 1px 0 rgba(255, 255, 255, 0.12);
    }

    .shell {
      padding: 20px;
      color: var(--primary-text-color);
    }

    .header,
    .dialog-head,
    .alarm-actions,
    .pill-row,
    .actions,
    .dialog-actions {
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: 12px;
    }

    .header {
      align-items: flex-start;
      margin-bottom: 18px;
    }

    .eyebrow,
    .subtle,
    .meta-grid span,
    .result span,
    .label {
      color: var(--secondary-text-color);
    }

    h1,
    h2 {
      margin: 4px 0;
      font-size: 1.4rem;
      line-height: 1.1;
    }

    button,
    input,
    select,
    textarea {
      font: inherit;
    }

    button {
      border: none;
      border-radius: 999px;
      padding: 10px 16px;
      cursor: pointer;
      transition: transform 120ms ease, opacity 120ms ease, background 120ms ease;
    }

    button:hover {
      transform: translateY(-1px);
    }

    button:disabled {
      opacity: 0.55;
      cursor: default;
      transform: none;
    }

    .primary {
      background: var(--primary-color);
      color: var(--text-primary-color, white);
    }

    .soft,
    .icon {
      background: color-mix(in srgb, var(--secondary-background-color, var(--card-background-color)) 82%, transparent);
      color: var(--primary-text-color);
    }

    .danger {
      background: color-mix(in srgb, var(--error-color) 18%, transparent);
      color: var(--error-color);
    }

    .state,
    .banner {
      border-radius: 22px;
      padding: 14px 16px;
      margin-bottom: 16px;
    }

    .state {
      text-align: center;
      background: color-mix(in srgb, var(--secondary-background-color, var(--card-background-color)) 80%, transparent);
    }

    .banner.success {
      background: color-mix(in srgb, var(--success-color, #2e7d32) 18%, transparent);
      color: var(--success-color, #2e7d32);
    }

    .banner.error {
      background: color-mix(in srgb, var(--error-color) 14%, transparent);
      color: var(--primary-text-color);
    }

    .list {
      display: grid;
      gap: 14px;
    }

    .alarm {
      display: grid;
      gap: 16px;
      padding: 16px;
      border-radius: 26px;
      background:
        linear-gradient(
          180deg,
          color-mix(in srgb, var(--card-background-color) 88%, white 12%),
          color-mix(in srgb, var(--card-background-color) 98%, black 2%)
        );
      box-shadow:
        inset 0 1px 0 rgba(255, 255, 255, 0.14),
        0 10px 24px rgba(0, 0, 0, 0.08);
    }

    .alarm-main {
      display: grid;
      gap: 12px;
    }

    .time-pill,
    .toggle {
      display: inline-flex;
      align-items: center;
      gap: 8px;
      border-radius: 999px;
      padding: 8px 12px;
      background: color-mix(in srgb, var(--primary-color) 14%, transparent);
    }

    .time-pill {
      font-size: 1.15rem;
      font-weight: 700;
      letter-spacing: 0.04em;
    }

    .toggle input {
      accent-color: var(--primary-color);
    }

    .alarm-title {
      font-size: 1.1rem;
      font-weight: 600;
    }

    .alarm-description {
      color: var(--secondary-text-color);
    }

    .meta-grid {
      display: grid;
      grid-template-columns: repeat(2, minmax(0, 1fr));
      gap: 12px;
    }

    .meta-grid div,
    .result {
      display: grid;
      gap: 4px;
      padding: 12px 14px;
      border-radius: 18px;
      background: color-mix(in srgb, var(--secondary-background-color, var(--card-background-color)) 74%, transparent);
    }

    .overlay {
      position: fixed;
      inset: 0;
      display: grid;
      place-items: center;
      padding: 16px;
      background: rgba(0, 0, 0, 0.4);
      z-index: 10;
    }

    .dialog {
      width: min(560px, 100%);
      max-height: min(92vh, 880px);
      overflow: auto;
      border-radius: 30px;
      padding: 20px;
      background: var(--card-background-color);
      box-shadow: 0 24px 60px rgba(0, 0, 0, 0.28);
    }

    form,
    .field,
    .chips {
      display: grid;
      gap: 10px;
    }

    .form-grid {
      display: grid;
      grid-template-columns: repeat(2, minmax(0, 1fr));
      gap: 12px;
    }

    input:not([type="checkbox"]),
    select,
    textarea {
      width: 100%;
      box-sizing: border-box;
      border: 1px solid color-mix(in srgb, var(--divider-color) 72%, transparent);
      border-radius: 18px;
      padding: 12px 14px;
      background: color-mix(in srgb, var(--secondary-background-color, var(--card-background-color)) 80%, transparent);
      color: var(--primary-text-color);
    }

    textarea {
      resize: vertical;
    }

    .checkbox-row,
    .chip {
      display: inline-flex;
      align-items: center;
      gap: 8px;
    }

    .chips {
      grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
    }

    .chip {
      justify-content: center;
      border-radius: 999px;
      padding: 10px 12px;
      background: color-mix(in srgb, var(--secondary-background-color, var(--card-background-color)) 82%, transparent);
    }

    .chip.selected {
      background: color-mix(in srgb, var(--primary-color) 22%, transparent);
    }

    .chip input {
      accent-color: var(--primary-color);
    }

    .field-error {
      color: var(--error-color);
      font-size: 0.9rem;
    }

    @media (max-width: 640px) {
      .shell,
      .dialog {
        padding: 16px;
      }

      .header,
      .alarm-actions,
      .actions,
      .dialog-head,
      .dialog-actions,
      .pill-row {
        flex-direction: column;
        align-items: stretch;
      }

      .meta-grid,
      .form-grid {
        grid-template-columns: 1fr;
      }

      button {
        width: 100%;
      }
    }
  `;
g([
  Y({ attribute: !1 })
], u.prototype, "hass", 2);
g([
  _()
], u.prototype, "_config", 2);
g([
  _()
], u.prototype, "_alarms", 2);
g([
  _()
], u.prototype, "_meta", 2);
g([
  _()
], u.prototype, "_translations", 2);
g([
  _()
], u.prototype, "_repeatModes", 2);
g([
  _()
], u.prototype, "_weekdays", 2);
g([
  _()
], u.prototype, "_loading", 2);
g([
  _()
], u.prototype, "_busy", 2);
g([
  _()
], u.prototype, "_dialogOpen", 2);
g([
  _()
], u.prototype, "_form", 2);
g([
  _()
], u.prototype, "_error", 2);
g([
  _()
], u.prototype, "_validation", 2);
g([
  _()
], u.prototype, "_statusMessage", 2);
u = g([
  Ae("wakemeup-card")
], u);
var rt = Object.defineProperty, it = Object.getOwnPropertyDescriptor, X = (r, e, t, s) => {
  for (var i = s > 1 ? void 0 : s ? it(e, t) : e, a = r.length - 1, o; a >= 0; a--)
    (o = r[a]) && (i = (s ? o(e, t, i) : o(i)) || i);
  return s && i && rt(e, t, i), i;
};
let j = class extends k {
  setConfig(r) {
    this._config = r;
  }
  render() {
    return this._config ? h`
      <div class="editor">
        <label>
          <span>Title</span>
          <input
            .value=${this._config.title ?? ""}
            @input=${this._onInput}
            data-field="title"
            placeholder="WakeMeUp"
          />
        </label>
      </div>
    ` : h``;
  }
  _onInput(r) {
    const e = r.currentTarget, t = e.dataset.field, s = {
      ...this._config,
      type: "custom:wakemeup-card",
      [t]: e.value.trim() || void 0
    };
    this._config = s, this.dispatchEvent(
      new CustomEvent("config-changed", {
        detail: { config: s }
      })
    );
  }
};
j.styles = _e`
    .editor {
      display: grid;
      gap: 16px;
    }

    label {
      display: grid;
      gap: 8px;
      color: var(--primary-text-color);
      font-size: 14px;
    }

    input {
      border: 1px solid var(--divider-color);
      border-radius: 16px;
      padding: 12px 14px;
      background: var(--card-background-color);
      color: var(--primary-text-color);
      font: inherit;
    }
  `;
X([
  Y({ attribute: !1 })
], j.prototype, "hass", 2);
X([
  _()
], j.prototype, "_config", 2);
j = X([
  Ae("wakemeup-card-editor")
], j);
window.customCards = window.customCards || [];
window.customCards.push({
  type: "wakemeup-card",
  name: "WakeMeUp Card",
  description: "Bubble-inspired card for managing WakeMeUp alarms.",
  preview: !0
});
