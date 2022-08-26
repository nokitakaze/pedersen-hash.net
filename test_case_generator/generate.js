#!/usr/bin/env node
'use strict';

const circomlib = require('circomlib');
const snarkjs = require("snarkjs");
const bigInt = snarkjs.bigInt;
const crypto = require("crypto");

const fs = require('fs');

/** Compute pedersen hash */
const pedersenHash = (data) => circomlib.babyJub.unpackPoint(circomlib.pedersenHash.hash(data))[0];

/** BigNumber to hex string of specified length */
function toHex(number, length = 32) {
    const str = number instanceof Buffer ? number.toString('hex') : bigInt(number).toString(16);
    return '0x' + str.padStart(length * 2, '0');
}

/** Generate random number of specified byte length */
const rbigint = (nbytes) => snarkjs.bigInt.leBuff2int(crypto.randomBytes(nbytes));

/**
 * Create deposit object from secret and nullifier
 */
/*
function createDeposit({nullifier, secret}) {
    const deposit = {nullifier, secret};
    deposit.preimage = Buffer.concat([deposit.nullifier.leInt2Buff(31), deposit.secret.leInt2Buff(31)]);

    const packedPoint = circomlib.pedersenHash.hash(deposit.preimage);
    const unpacked = circomlib.babyJub.unpackPoint(packedPoint);
    console.log('packedPoint', packedPoint);
    console.log('unpacked', unpacked);

    deposit.commitment = pedersenHash(deposit.preimage);
    if (deposit.commitment !== unpacked[0]) {
        console.error('commitment ', deposit.commitment, ' != unpacked ', unpacked[0]);
        throw new Error();
    }

    deposit.commitmentHex = toHex(deposit.commitment);
    deposit.nullifierHash = pedersenHash(deposit.nullifier.leInt2Buff(31));
    deposit.nullifierHex = toHex(deposit.nullifierHash);
    return deposit;
}

const deposit = createDeposit({
    nullifier: rbigint(31),
    secret: rbigint(31)
});


console.log('commitment', deposit.commitment, '\nnullifier', deposit.nullifier, '\nsecret', deposit.secret);
*/

const F1Field = require("./node_modules/snarkjs/src/zqfield.js");
// const F2Field = require("./node_modules/snarkjs/src/f2field.js");
// const F3Field = require("./node_modules/snarkjs/src/f3field.js");
// const GCurve = require("./node_modules/snarkjs/src/gcurve.js");

async function readPrimaries() {
    const contents = await fs.promises.readFile('prime.txt', 'utf-8');
    return contents.split("\n").map(line => line.trimEnd()).filter(x => x !== '');
}

readPrimaries().then(async primaries => {
    // console.log(primaries);

    let F1FieldText = '';
    for (let i = 0; i < 10; i++) {
        const id = Math.floor(Math.random() * primaries.length);
        const primary = primaries[id];
        const bigNumber = bigInt(primary);
        const f1 = new F1Field(bigNumber);

        // twoinv nqr t nqr_to_t
        const s = `${primary}\t${f1.twoinv}\t${f1.nqr}\t${f1.t}\t${f1.nqr_to_t}`;
        // console.log(s);
        F1FieldText += s + '\n';
    }

    // save test data to file
    await fs.promises.writeFile('test-f1.tsv', F1FieldText);

    /*
    const nonResidueF2 = bigInt("21888242871839275222246405745257275088696311157297823662689037894645226208582");
    const nonResidueF6 = [ bigInt("9"), bigInt("1") ];

    let F2FieldText = '';
    for (let i = 0; i < 10; i++) {
        // number #1
        const id1 = Math.floor(Math.random() * primaries.length);
        const primary1 = primaries[id1];

        const big1 = bigInt(primary1);
        const f1 = new F1Field(big1);
        const f2a = new F2Field(f1, nonResidueF2);

        // number #2
        const id2 = Math.floor(Math.random() * primaries.length);
        const primary2 = primaries[id2];

        const big2 = bigInt(primary2);
        const f2 = new F1Field(big2);
        const f2b = new F2Field(f2, nonResidueF2);

        //
        const f2c = f2a.add(f2b);

        // twoinv nqr t nqr_to_t
        const s = `${primary1}\t${f1.twoinv}\t${f1.nqr}\t${f1.t}\t${f1.nqr_to_t}`;
        F2FieldText += s + '\n';
    }
    await fs.promises.writeFile('test-f2.tsv', F2FieldText);
    */
})

