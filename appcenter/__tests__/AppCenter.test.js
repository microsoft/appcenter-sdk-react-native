// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
import { CustomProperties } from '../AppCenter';

it('CustomProperties set numbers correctly', () => {
    const properties = new CustomProperties()
        .set('zero', 0)
        .set('pi', 3.14)
        .set('score', -7);
    expect(properties.zero).toEqual({ type: 'number', value: 0 });
    expect(properties.pi).toEqual({ type: 'number', value: 3.14 });
    expect(properties.score).toEqual({ type: 'number', value: -7 });
});

it('CustomProperties set strings correctly', () => {
    const properties = new CustomProperties()
        .set('empty-string', '')
        .set('color', 'blue');
    expect(properties['empty-string']).toEqual({ type: 'string', value: '' });
    expect(properties.color).toEqual({ type: 'string', value: 'blue' });
});

it('CustomProperties set booleans correctly', () => {
    const properties = new CustomProperties()
        .set('optin', true)
        .set('optout', false);
    expect(properties.optin).toEqual({ type: 'boolean', value: true });
    expect(properties.optout).toEqual({ type: 'boolean', value: false });
});

it('CustomProperties set dates correctly', () => {
    const properties = new CustomProperties()
        .set('now', new Date());
    expect(properties.now.type).toBe('date-time');
});

it('CustomProperties does not throw on invalid property key type. Only string type is allowed.', () => {
    jest.unmock('../appcenter-log');
    const AppCenterLog = require('../appcenter-log');
    AppCenterLog.error = jest.fn(() => Promise.resolve());
    const properties = new CustomProperties()
        .set(undefined, 'foo')
        .set({}, 'foo2');
    expect(properties).toEqual({});
});

it('CustomProperties does not throw on invalid property value type. Only string|number|boolean|Date type is allowed.', () => {
    jest.unmock('../appcenter-log');
    const AppCenterLog = require('../appcenter-log');
    AppCenterLog.error = jest.fn(() => Promise.resolve());
    const properties = new CustomProperties()
        .set('foo', { foo: 'bar' });
    expect(properties).toEqual({});
});

it('CustomProperties clear works correctly', () => {
    const properties = new CustomProperties()
        .clear('color', 'blue');
    expect(properties.color).toEqual({ type: 'clear' });
});

